using System;
using System.Collections.Generic;
using System.Text;

namespace WurthPlanner.Models
{
    public class WorkItemReference(Guid id, string title, WorkItemType type)
    {
        public Guid Id { get; } = id;
        public string Title { get; } = title;
        public WorkItemType Type { get; } = type;
        public string? Excerpt { get; private set; }
        public ExternalReference? Source { get; private set; }

        public static implicit operator WorkItemReference(WorkItem workItem)
        {
            return new WorkItemReference(workItem.Id, workItem.Title, workItem.Type)
            {
                Source = workItem.Source,
                Excerpt = workItem.Excerpt
            };
        }
    }
}
