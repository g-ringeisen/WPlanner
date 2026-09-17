using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Reduced read model containing the information needed to identify and display a work item.
    /// This model is intended for lists, planning views, search results, and other lightweight views.
    /// </summary>
    public interface IWorkItemBrief
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
        /// Gets the type of the work item.
        /// </summary>
        WorkItemType Type { get; }

        /// <summary>
        /// Gets a short excerpt of the work item content, when available.
        /// </summary>
        string? Excerpt { get; }

        /// <summary>
        /// Gets the external reference associated with the work item, when available.
        /// </summary>
        IExternalReference? Source { get; }
    }
}
