using System;
using WurthPlanner.Contracts.Common;

namespace WurthPlanner.Contracts.Notes;

public sealed record NoteSearchRequest : PageRequest
{
    public Guid? WorkItemId { get; init; }
    public string? SearchText { get; init; }
    public NoteSortField SortBy { get; init; } = NoteSortField.CreatedAt;
    public SortDirection SortDirection { get; init; } = SortDirection.Ascending;
}

public enum NoteSortField { Title, CreatedAt, UpdatedAt }
