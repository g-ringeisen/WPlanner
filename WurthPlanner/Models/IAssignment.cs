using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Read model representing one planned allocation of a work item to an assignee.
    /// Multiple assignments may exist for the same work item and assignee when the work
    /// is planned over separate periods.
    /// </summary>
    public interface IAssignment : IAuditable
    {
        /// <summary>
        /// Gets the unique identifier of the assignment.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the identifier of the related work item.
        /// The work item details are loaded separately through this identifier.
        /// </summary>
        Guid WorkItemId { get; }

        /// <summary>
        /// Gets the identifier of the assigned person.
        /// This value may be null when the planning entry is not assigned yet.
        /// </summary>
        string? AssigneeId { get; }

        /// <summary>
        /// Gets the instructions associated with this assignment.
        /// </summary>
        ITextContent Instruction { get; }

        /// <summary>
        /// Gets a short excerpt of the assignment instructions.
        /// </summary>
        string? Excerpt => Instruction.Excerpt;

        /// <summary>
        /// Gets the planned effort in person-days for this assignment.
        /// Decimal values allow partial days, such as 0.5 day.
        /// </summary>
        decimal? PlannedDays { get; }

        /// <summary>
        /// Gets the first planned day of the assignment.
        /// </summary>
        DateOnly? StartDate { get; }

        /// <summary>
        /// Gets the last planned day of the assignment.
        /// The planning range is expressed at day level.
        /// </summary>
        DateOnly? EndDate { get; }
    }
}
