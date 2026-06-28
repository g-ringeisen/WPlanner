using WurthPlanner.Models;

namespace WurthPlannerWebview.Utils
{
    public class WorkItemTypeInfo
    {
        public static string Label(WorkItemType type) => type switch
        {
            WorkItemType.Unknown => "Inconnu",
            WorkItemType.Task => "Tâche",
            WorkItemType.Phase => "Phase",
            WorkItemType.Event => "Événement",
            WorkItemType.Note => "Note",
            WorkItemType.Assignment => "Affectation",
            _ => type.ToString()
        };

        public static string Icon(WorkItemType type) => type switch
        {
            WorkItemType.Unknown => "bi-person",
            WorkItemType.Task => "bi-check-square",
            WorkItemType.Phase => "bi-rocket",
            WorkItemType.Event => "bi-people",
            WorkItemType.Note => "bi-file-text",
            WorkItemType.Assignment => "bi-clipboard-check",
            _ => "bi-file-text"
        };

        public static IReadOnlyList<WorkItemType> All { get; } =
            Enum.GetValues<WorkItemType>();
    }
}
