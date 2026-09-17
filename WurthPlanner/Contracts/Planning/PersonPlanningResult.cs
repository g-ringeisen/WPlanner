using System;
using System.Collections.Generic;
using WurthPlanner.Models;

namespace WurthPlanner.Contracts.Planning;

public sealed record PersonPlanningResult(
    string EmployeeId,
    DateOnly From,
    DateOnly To,
    IReadOnlyCollection<PlannedAssignmentResult> Assignments);

public sealed record PlannedAssignmentResult(
    IAssignment Assignment,
    IWorkItemBrief WorkItem);
