using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Instantiable lightweight work item model for lists and planning views.
    /// </summary>
    public class WorkItemBrief : IWorkItemBrief
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public WorkItemStatus Status { get; set; }
        public WorkItemType Type { get; set; }
        public string? Excerpt { get; set; }
        public IExternalReference? Source { get; set; }
    }
}
