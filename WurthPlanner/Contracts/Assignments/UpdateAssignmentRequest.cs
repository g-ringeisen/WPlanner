using WurthPlanner.Contracts.Content;

namespace WurthPlanner.Contracts.Assignments;

public sealed record UpdateAssignmentRequest(
    TextContentInput Instruction,
    string? AssigneeId,
    decimal? PlannedDays,
    DateOnly? StartDate,
    DateOnly? EndDate);
