using System;
using System.Collections.Generic;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Detailed read model representing a work item.
    /// Assignments and notes are loaded separately through their related queries.
    /// </summary>
    public interface IWorkItem : IAuditable
    {
        /// <summary>
        /// Gets the unique identifier of the work item.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the title of the work item.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the current status of the work item.
        /// </summary>
        WorkItemStatus Status { get; }

        /// <summary>
        /// Gets the type of the work item, such as task, meeting, phase, or project.
        /// </summary>
        WorkItemType Type { get; }

        /// <summary>
        /// Gets the description of the work item.
        /// </summary>
        ITextContent Description { get; }

        /// <summary>
        /// Gets a short excerpt of the work item description.
        /// </summary>
        string? Excerpt => Description.Excerpt;

        /// <summary>
        /// Gets the tags associated with the work item.
        /// </summary>
        IReadOnlyCollection<string> Tags { get; }

        /// <summary>
        /// Gets the due date of the work item, when one has been defined.
        /// </summary>
        DateOnly? DueDate { get; }

        /// <summary>
        /// Gets the estimated effort in person-days.
        /// Decimal values allow partial days.
        /// </summary>
        decimal? EffortDays { get; }

        /// <summary>
        /// Gets the identifier of the parent work item, when the item belongs to a hierarchy.
        /// </summary>
        Guid? ParentId { get; }

        /// <summary>
        /// Gets the external reference associated with the work item, when available.
        /// </summary>
        IExternalReference? Source { get; }
    }
}
