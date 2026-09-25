namespace WurthPlannerWebview.Utils;

/// <summary>Utilitaires de calendrier indépendants de la persistence.</summary>
public static class PlanningLayout
{
    public static bool IsWeekend(DateOnly date) =>
        date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public static DateOnly GetWeekStart(DateOnly date) =>
        date.AddDays(-((int)date.DayOfWeek + 6) % 7);
}
