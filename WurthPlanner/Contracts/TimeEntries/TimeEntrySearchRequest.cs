using System;
using WurthPlanner.Contracts.Common;

namespace WurthPlanner.Contracts.TimeEntries;

public sealed record TimeEntrySearchRequest : PageRequest
{
    public Guid? WorkItemId { get; init; }
    public Guid? AssignmentId { get; init; }
    public string? EmployeeId { get; init; }
    public DateOnly? WorkDateFrom { get; init; }
    public DateOnly? WorkDateTo { get; init; }
    public TimeEntrySortField SortBy { get; init; } = TimeEntrySortField.WorkDate;
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;
}

public enum TimeEntrySortField { WorkDate, ActualHours, CreatedAt, UpdatedAt }
