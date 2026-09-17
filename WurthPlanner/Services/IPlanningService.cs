using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WurthPlanner.Contracts.Planning;

namespace WurthPlanner.Services;

/// <summary>Provides read-only operational views of planned work, actual time and planning exceptions.</summary>
/// <remarks>This service reports total planned workload and dates. It does not automatically smooth or distribute workload across days.</remarks>
public interface IPlanningService
{
    /// <summary>Gets the planned assignments of one employee over an inclusive date range.</summary>
    /// <param name="employeeId">The stable employee identifier.</param>
    /// <param name="from">The first included operational day.</param>
    /// <param name="to">The last included operational day.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The planning projection for the requested employee.</returns>
    Task<PersonPlanningResult> GetPersonPlanningAsync(string employeeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);

    /// <summary>Gets the operational planning of the employees and period specified by the query.</summary>
    /// <param name="query">The selected employees and inclusive date range.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A planning projection for each selected employee.</returns>
    Task<IReadOnlyCollection<PersonPlanningResult>> GetPlanningAsync(PlanningQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets the estimated, planned and actual workload summary of a work item.</summary>
    /// <param name="workItemId">The work item identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The planning summary, or <see langword="null"/> when the work item does not exist.</returns>
    Task<WorkItemPlanningSummary?> GetWorkItemSummaryAsync(Guid workItemId, CancellationToken cancellationToken = default);

    /// <summary>Gets operational exceptions detected for the selected people and period.</summary>
    /// <param name="query">The selected employees and inclusive date range.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>Detected overdue, unplanned, unassigned or schedule-variance work items.</returns>
    Task<IReadOnlyCollection<PlanningException>> GetExceptionsAsync(PlanningQuery query, CancellationToken cancellationToken = default);
}
