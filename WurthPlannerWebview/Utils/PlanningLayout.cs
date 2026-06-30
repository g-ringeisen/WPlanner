using WurthPlanner.Models;

namespace WurthPlannerWebview.Utils;

/// <summary>
/// Calculs purs (sans dépendance UI) pour la page Planning :
/// - bornes de la plage de dates affichée
/// - détection week-end / jour férié
/// - empilement (packing) des Assignments qui se chevauchent dans une swimlane
/// </summary>
public static class PlanningLayout
{
    /// <summary>
    /// Jours fériés français fixes + mobiles calculés à la volée (calcul de Pâques
    /// par l'algorithme de Meeus/Jones/Butcher). Suffisant pour l'affichage du
    /// planning ; à remplacer par une source de données si besoin de précision
    /// juridique (jours fériés régionaux, etc.).
    /// </summary>
    public static bool IsPublicHoliday(DateOnly date)
    {
        int year = date.Year;

        var fixedHolidays = new[]
        {
            new DateOnly(year, 1, 1),   // Jour de l'an
            new DateOnly(year, 5, 1),   // Fête du travail
            new DateOnly(year, 5, 8),   // Victoire 1945
            new DateOnly(year, 7, 14),  // Fête nationale
            new DateOnly(year, 8, 15),  // Assomption
            new DateOnly(year, 11, 1),  // Toussaint
            new DateOnly(year, 11, 11), // Armistice
            new DateOnly(year, 12, 25), // Noël
        };

        if (fixedHolidays.Contains(date))
            return true;

        var easter = ComputeEasterSunday(year);
        var movableHolidays = new[]
        {
            easter.AddDays(1),  // Lundi de Pâques
            easter.AddDays(39), // Ascension
            easter.AddDays(50), // Lundi de Pentecôte
        };

        return movableHolidays.Contains(date);
    }

    private static DateOnly ComputeEasterSunday(int year)
    {
        int a = year % 19;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;
        return new DateOnly(year, month, day);
    }

    public static bool IsWeekend(DateOnly date) =>
        date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

    public static bool IsNonWorkingDay(DateOnly date) => IsWeekend(date) || IsPublicHoliday(date);

    /// <summary>
    /// Retourne le lundi de la semaine courante moins <paramref name="weeksBeforeToday"/> semaines.
    /// </summary>
    public static DateOnly GetWeekStart(DateOnly date)
    {
        int diff = ((int)date.DayOfWeek + 6) % 7; // lundi = 0
        return date.AddDays(-diff);
    }

    /// <summary>
    /// Une barre positionnée dans une swimlane : référence le WorkItem d'origine,
    /// ses bornes en jours (offsets depuis le début de plage affichée) et le
    /// numéro de "rangée" (0-based) au sein de la swimlane après packing.
    /// </summary>
    public record PlacedBar(WorkItem WorkItem, int StartDayOffset, int DurationDays, int Row);

    /// <summary>
    /// Place les WorkItems d'une swimlane sur des rangées de sorte que deux
    /// barres ne se chevauchant jamais ne partagent la même rangée. Algorithme
    /// glouton classique : on trie par date de début puis on assigne à la
    /// première rangée libre (dont la dernière barre se termine avant le
    /// début de la barre courante).
    /// </summary>
    public static List<PlacedBar> PackBars(IEnumerable<WorkItem> items, DateOnly rangeStart)
    {
        var ordered = items
            .Where(i => i.StartDate.HasValue && i.EndDate.HasValue)
            .OrderBy(i => i.StartDate)
            .ToList();

        var rowEndOffsets = new List<int>(); // dernier offset de fin (exclusif) occupé par rangée
        var placed = new List<PlacedBar>();

        foreach (var item in ordered)
        {
            var start = DateOnly.FromDateTime(item.StartDate!.Value);
            var end = DateOnly.FromDateTime(item.EndDate!.Value);

            int startOffset = start.DayNumber - rangeStart.DayNumber;
            int endOffset = Math.Max(end.DayNumber - rangeStart.DayNumber, startOffset + 1);
            int duration = endOffset - startOffset;

            int row = rowEndOffsets.FindIndex(rowEnd => rowEnd <= startOffset);
            if (row == -1)
            {
                row = rowEndOffsets.Count;
                rowEndOffsets.Add(endOffset);
            }
            else
            {
                rowEndOffsets[row] = endOffset;
            }

            placed.Add(new PlacedBar(item, startOffset, duration, row));
        }

        return placed;
    }
}
