using MongoDB.Driver;
using WurthPlanner.Contracts.Planning;
using WurthPlanner.Models;
using WurthPlanner.Services;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Builds read-only planning projections from work items, assignments and actual time entries.
/// </summary>
public sealed class MongoPlanningService : IPlanningService
{
    private readonly MongoDbContext _context;

    public MongoPlanningService(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<PersonPlanningResult> GetPersonPlanningAsync(
        string employeeId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        ValidatePeriod(from, to);
        ArgumentException.ThrowIfNullOrWhiteSpace(employeeId);

        var assignments = await _context.Assignments
            .Find(item => item.AssigneeId == employeeId
                && (item.EndDate == null || item.EndDate >= from)
                && (item.StartDate == null || item.StartDate <= to))
            .ToListAsync(cancellationToken);

        var workItemIds = assignments.Select(item => item.WorkItemId).Distinct().ToArray();
        var workItems = await _context.WorkItems
            .Find(item => workItemIds.Contains(item.Id))
            .ToListAsync(cancellationToken);

        var plannedAssignments = assignments
            .Join(
                workItems,
                assignment => assignment.WorkItemId,
                workItem => workItem.Id,
                (assignment, workItem) => new PlannedAssignmentResult(assignment, MongoMapping.ToBrief(workItem)))
            .ToArray();

        return new PersonPlanningResult(employeeId, from, to, plannedAssignments);
    }

    public async Task<IReadOnlyCollection<PersonPlanningResult>> GetPlanningAsync(
        PlanningQuery query,
        CancellationToken cancellationToken = default)
    {
        ValidatePeriod(query.From, query.To);

        var tasks = query.EmployeeIds
            .Distinct(StringComparer.Ordinal)
            .Select(employeeId => GetPersonPlanningAsync(employeeId, query.From, query.To, cancellationToken));

        return await Task.WhenAll(tasks);
    }

    public async Task<WorkItemPlanningSummary?> GetWorkItemSummaryAsync(
        Guid workItemId,
        CancellationToken cancellationToken = default)
    {
        var workItem = await _context.WorkItems
            .Find(item => item.Id == workItemId)
            .FirstOrDefaultAsync(cancellationToken);

        if (workItem is null) return null;

        var assignments = await _context.Assignments
            .Find(item => item.WorkItemId == workItemId)
            .ToListAsync(cancellationToken);

        var timeEntries = await _context.TimeEntries
            .Find(item => item.WorkItemId == workItemId)
            .ToListAsync(cancellationToken);

        return new WorkItemPlanningSummary(
            MongoMapping.ToBrief(workItem),
            workItem.EffortDays,
            assignments.Sum(item => item.PlannedDays ?? 0),
            timeEntries.Sum(item => item.ActualHours),
            workItem.DueDate);
    }

    public async Task<IReadOnlyCollection<PlanningException>> GetExceptionsAsync(
        PlanningQuery query,
        CancellationToken cancellationToken = default)
    {
        ValidatePeriod(query.From, query.To);

        var selectedEmployeeIds = query.EmployeeIds.Distinct(StringComparer.Ordinal).ToArray();
        var assignments = await _context.Assignments
            .Find(item => selectedEmployeeIds.Contains(item.AssigneeId!)
                && (item.EndDate == null || item.EndDate >= query.From)
                && (item.StartDate == null || item.StartDate <= query.To))
            .ToListAsync(cancellationToken);

        var workItemIds = assignments.Select(item => item.WorkItemId).Distinct().ToArray();
        var workItems = await _context.WorkItems.Find(item => workItemIds.Contains(item.Id)).ToListAsync(cancellationToken);
        var exceptions = new List<PlanningException>();

        foreach (var workItem in workItems)
        {
            var relatedAssignments = assignments.Where(item => item.WorkItemId == workItem.Id).ToArray();
            var brief = MongoMapping.ToBrief(workItem);

            if (workItem.DueDate < query.From
                && workItem.Status is not WorkItemStatus.Closed and not WorkItemStatus.Cancelled)
            {
                exceptions.Add(new PlanningException(PlanningExceptionType.Overdue, brief, "The due date has passed."));
            }

            if (relatedAssignments.Length == 0)
            {
                exceptions.Add(new PlanningException(PlanningExceptionType.Unassigned, brief, "No selected employee has a planned assignment."));
            }
            else if (relatedAssignments.All(item => item.PlannedDays is null or 0))
            {
                exceptions.Add(new PlanningException(PlanningExceptionType.Unplanned, brief, "No planned effort is defined."));
            }
        }

        return exceptions;
    }

    private static void ValidatePeriod(DateOnly from, DateOnly to)
    {
        if (from > to) throw new ArgumentException("The start date must not be after the end date.");
    }
}
