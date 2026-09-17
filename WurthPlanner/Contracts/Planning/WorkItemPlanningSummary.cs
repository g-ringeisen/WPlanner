using System;
using WurthPlanner.Models;

namespace WurthPlanner.Contracts.Planning;

public sealed record WorkItemPlanningSummary(
    IWorkItemBrief WorkItem,
    decimal? EstimatedDays,
    decimal PlannedDays,
    decimal ActualHours,
    DateOnly? DueDate);
