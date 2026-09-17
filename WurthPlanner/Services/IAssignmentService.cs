using System;
using System.Threading;
using System.Threading.Tasks;
using WurthPlanner.Contracts.Assignments;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Contracts.TimeEntries;
using WurthPlanner.Models;

namespace WurthPlanner.Services;

/// <summary>
/// Provides application operations for planned allocations of work items to people.
/// </summary>
/// <remarks>
/// Planning is expressed at day level. More than one assignment may link the same person to the
/// same work item when the work is planned across distinct periods.
/// </remarks>
public interface IAssignmentService
{
    /// <summary>Creates a planned assignment.</summary>
    /// <param name="request">The work item, assignee, planning period and planned effort.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The newly created assignment.</returns>
    Task<IAssignment> CreateAsync(CreateAssignmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets an assignment by its identifier.</summary>
    /// <param name="id">The assignment identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The assignment, or <see langword="null"/> when it does not exist.</returns>
    Task<IAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing planned assignment.</summary>
    /// <param name="id">The assignment identifier.</param>
    /// <param name="request">The replacement planning values.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The updated assignment.</returns>
    /// <remarks>The implementation must validate that the start date is not after the end date and that planned days are positive when specified.</remarks>
    Task<IAssignment> UpdateAsync(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes an assignment that has no linked time entries.</summary>
    /// <param name="id">The assignment identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <remarks>The implementation must refuse the operation when time entries are linked to the assignment.</remarks>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Searches planned assignments by work item, assignee, planning overlap and pagination.</summary>
    /// <param name="request">The search criteria and paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of assignments matching the criteria.</returns>
    Task<PagedResult<IAssignment>> SearchAsync(AssignmentSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the lightweight work item associated with an assignment.</summary>
    /// <param name="id">The assignment identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The related work item, or <see langword="null"/> when the assignment does not exist.</returns>
    Task<IWorkItemBrief?> GetWorkItemAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets the actual time entries linked to an assignment.</summary>
    /// <param name="id">The assignment identifier.</param>
    /// <param name="request">Optional time-entry filters and paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of linked time entries.</returns>
    Task<PagedResult<ITimeEntry>> GetTimeEntriesAsync(Guid id, TimeEntrySearchRequest request, CancellationToken cancellationToken = default);
}
