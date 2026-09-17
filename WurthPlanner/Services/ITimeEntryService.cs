using System;
using System.Threading;
using System.Threading.Tasks;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Contracts.TimeEntries;
using WurthPlanner.Models;

namespace WurthPlanner.Services;

/// <summary>Provides application operations for actual time recorded against work items.</summary>
/// <remarks>Time is recorded at operational-day granularity. A time entry may exist without a related planned assignment.</remarks>
public interface ITimeEntryService
{
    /// <summary>Creates a time entry.</summary>
    /// <param name="request">The work performed, employee, operational day, duration and optional related assignment.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The newly created time entry.</returns>
    /// <remarks>The implementation must ensure that actual hours are strictly positive and that any specified assignment belongs to the same work item.</remarks>
    Task<ITimeEntry> CreateAsync(CreateTimeEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a time entry by its identifier.</summary>
    /// <param name="id">The time-entry identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The time entry, or <see langword="null"/> when it does not exist.</returns>
    Task<ITimeEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Updates a time entry.</summary>
    /// <param name="id">The time-entry identifier.</param>
    /// <param name="request">The replacement values for the time entry.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The updated time entry.</returns>
    Task<ITimeEntry> UpdateAsync(Guid id, UpdateTimeEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a time entry.</summary>
    /// <param name="id">The time-entry identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Searches time entries by work item, employee, assignment, work-date interval and pagination.</summary>
    /// <param name="request">The search criteria and paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of matching time entries.</returns>
    Task<PagedResult<ITimeEntry>> SearchAsync(TimeEntrySearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the lightweight work item associated with a time entry.</summary>
    /// <param name="id">The time-entry identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The related work item, or <see langword="null"/> when the entry does not exist.</returns>
    Task<IWorkItemBrief?> GetWorkItemAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets the planned assignment associated with a time entry when one is defined.</summary>
    /// <param name="id">The time-entry identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The related assignment, or <see langword="null"/> when there is none or the entry does not exist.</returns>
    Task<IAssignment?> GetAssignmentAsync(Guid id, CancellationToken cancellationToken = default);
}
