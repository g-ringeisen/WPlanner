using System;
using System.Collections.Generic;
using WurthPlanner.Contracts.Content;
using WurthPlanner.Models;

namespace WurthPlanner.Contracts.WorkItems;

public sealed record UpdateWorkItemRequest(
    string Title,
    WorkItemStatus Status,
    WorkItemType Type,
    TextContentInput Description,
    IReadOnlyCollection<string>? Tags,
    DateOnly? DueDate,
    decimal? EffortDays,
    Guid? ParentId,
    ExternalReferenceInput? Source);
