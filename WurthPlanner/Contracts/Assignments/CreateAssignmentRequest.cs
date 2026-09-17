using System;
using WurthPlanner.Contracts.Content;

namespace WurthPlanner.Contracts.Assignments;

public sealed record CreateAssignmentRequest(
    Guid WorkItemId,
    TextContentInput Instruction,
    string? AssigneeId = null,
    decimal? PlannedDays = null,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null);
