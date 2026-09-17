using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Read model representing time actually spent by one employee on one work item,
    /// for one operational day.
    /// </summary>
    public interface ITimeEntry : IAuditable
    {
        /// <summary>
        /// Gets the unique identifier of the time entry.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the identifier of the work item on which time was spent.
        /// </summary>
        Guid WorkItemId { get; }

        /// <summary>
        /// Gets the identifier of the employee who recorded the time.
        /// This identifier refers to the employee directory, not necessarily
        /// to the authenticated user account.
        /// </summary>
        string EmployeeId { get; }

        /// <summary>
        /// Gets the identifier of the planned assignment this entry relates to,
        /// when the work was performed against a scheduled allocation.
        /// </summary>
        Guid? AssignmentId { get; }

        /// <summary>
        /// Gets the operational day on which the work was carried out.
        /// The planner intentionally works at day granularity.
        /// </summary>
        DateOnly WorkDate { get; }

        /// <summary>
        /// Gets the actual duration spent, expressed in hours.
        /// Must be strictly greater than zero.
        /// </summary>
        decimal ActualHours { get; }

        /// <summary>
        /// Gets an optional comment explaining the work performed,
        /// a discrepancy, or any relevant context.
        /// </summary>
        ITextContent? Comment { get; }

        /// <summary>
        /// Gets a short excerpt of the optional comment.
        /// </summary>
        string? Excerpt => Comment?.Excerpt;
    }
}
