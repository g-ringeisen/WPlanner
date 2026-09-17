using System;
using System.Collections.Generic;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Instantiable detailed work item model.
    /// </summary>
    public class WorkItem : Auditable, IWorkItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public WorkItemStatus Status { get; set; }
        public WorkItemType Type { get; set; }
        public ITextContent Description { get; set; } = new TextContent();
        public string? Excerpt => Description.Excerpt;
        public IReadOnlyCollection<string> Tags { get; set; } = Array.Empty<string>();
        public DateOnly? DueDate { get; set; }
        public decimal? EffortDays { get; set; }
        public Guid? ParentId { get; set; }
        public IExternalReference? Source { get; set; }
    }
}
