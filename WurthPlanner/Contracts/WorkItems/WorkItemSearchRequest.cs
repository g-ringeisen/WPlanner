using System;
using System.Collections.Generic;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Models;

namespace WurthPlanner.Contracts.WorkItems;

public sealed record WorkItemSearchRequest : PageRequest
{
    public string? SearchText { get; init; }
    public IReadOnlyCollection<WorkItemStatus>? Statuses { get; init; }
    public IReadOnlyCollection<WorkItemType>? Types { get; init; }
    public IReadOnlyCollection<string>? Tags { get; init; }
    public Guid? ParentId { get; init; }
    public bool? HasParent { get; init; }
    public DateOnly? DueFrom { get; init; }
    public DateOnly? DueTo { get; init; }
    public string? CreatedBy { get; init; }
    public string? SourceSystem { get; init; }
    public WorkItemSortField SortBy { get; init; } = WorkItemSortField.UpdatedAt;
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;
}

public enum WorkItemSortField { Title, Status, Type, DueDate, CreatedAt, UpdatedAt }
