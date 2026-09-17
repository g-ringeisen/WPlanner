using WurthPlanner.Models;

namespace WurthPlanner.Contracts.Planning;

public enum PlanningExceptionType { Overdue, Unplanned, Unassigned, ScheduleVariance }

public sealed record PlanningException(
    PlanningExceptionType Type,
    IWorkItemBrief WorkItem,
    string Description);
