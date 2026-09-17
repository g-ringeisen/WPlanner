using System;
using System.Collections.Generic;
using WurthPlanner.Contracts.Content;
using WurthPlanner.Models;

namespace WurthPlanner.Contracts.WorkItems;

public sealed record CreateWorkItemRequest(
    string Title,
    WorkItemType Type,
    TextContentInput Description,
    WorkItemStatus? Status = null,
    IReadOnlyCollection<string>? Tags = null,
    DateOnly? DueDate = null,
    decimal? EffortDays = null,
    Guid? ParentId = null,
    ExternalReferenceInput? Source = null);
