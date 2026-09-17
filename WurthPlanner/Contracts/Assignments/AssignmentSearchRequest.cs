using System;
using WurthPlanner.Contracts.Common;

namespace WurthPlanner.Contracts.Assignments;

public sealed record AssignmentSearchRequest : PageRequest
{
    public Guid? WorkItemId { get; init; }
    public string? AssigneeId { get; init; }
    public bool? IsUnassigned { get; init; }
    /// <summary>Returns assignments whose inclusive planned period overlaps this interval.</summary>
    public DateOnly? OverlapFrom { get; init; }
    public DateOnly? OverlapTo { get; init; }
    public AssignmentSortField SortBy { get; init; } = AssignmentSortField.StartDate;
    public SortDirection SortDirection { get; init; } = SortDirection.Ascending;
}

public enum AssignmentSortField { StartDate, EndDate, PlannedDays, CreatedAt, UpdatedAt }
