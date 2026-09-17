using System;
using System.Collections.Generic;

namespace WurthPlanner.Contracts.Planning;

/// <summary>Inclusive date range and selected employees for an operational planning view.</summary>
public sealed record PlanningQuery(
    DateOnly From,
    DateOnly To,
    IReadOnlyCollection<string> EmployeeIds);
