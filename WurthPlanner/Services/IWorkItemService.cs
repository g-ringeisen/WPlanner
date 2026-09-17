using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WurthPlanner.Contracts.Assignments;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Contracts.Notes;
using WurthPlanner.Contracts.TimeEntries;
using WurthPlanner.Contracts.WorkItems;
using WurthPlanner.Models;

namespace WurthPlanner.Services;

/// <summary>
/// Provides application operations for the lifecycle, hierarchy and related data of work items.
/// </summary>
/// <remarks>
/// Related collections are deliberately loaded through dedicated methods to avoid implicit loading
/// of assignments, notes and time entries when a work item is retrieved.
/// </remarks>
public interface IWorkItemService
{
    /// <summary>Creates a new work item.</summary>
    /// <param name="request">The functional information used to create the work item.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The newly created work item.</returns>
    Task<IWorkItem> CreateAsync(CreateWorkItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the detailed work item identified by the specified identifier.</summary>
    /// <param name="id">The work item identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The work item, or <see langword="null"/> when it does not exist.</returns>
    Task<IWorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Updates the functional information of an existing work item.</summary>
    /// <param name="id">The work item identifier.</param>
    /// <param name="request">The replacement values for the work item.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The updated work item.</returns>
    /// <remarks>The implementation must reject hierarchy cycles, including self-parenting.</remarks>
    Task<IWorkItem> UpdateAsync(Guid id, UpdateWorkItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a work item that has no protected dependent data.</summary>
    /// <param name="id">The work item identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <remarks>
    /// The implementation must refuse the operation when children, assignments, notes or time entries
    /// still exist. It must not silently delete dependent data.
    /// </remarks>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Searches work items by text, functional filters, sorting and pagination.</summary>
    /// <param name="request">The search criteria and paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of lightweight work-item models.</returns>
    Task<PagedResult<IWorkItemBrief>> SearchAsync(WorkItemSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the direct parent of a work item.</summary>
    /// <param name="id">The child work item identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The direct parent, or <see langword="null"/> if the item has no parent or does not exist.</returns>
    Task<IWorkItemBrief?> GetParentAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets the hierarchy path from the root work item to the specified work item.</summary>
    /// <param name="id">The work item identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>An ordered collection containing the ancestors of the work item.</returns>
    Task<IReadOnlyCollection<IWorkItemBrief>> GetAncestorPathAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets the direct children of a work item.</summary>
    /// <param name="id">The parent work item identifier.</param>
    /// <param name="request">The paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of direct child work items.</returns>
    Task<PagedResult<IWorkItemBrief>> GetChildrenAsync(Guid id, PageRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the planned assignments associated with a work item.</summary>
    /// <param name="id">The work item identifier.</param>
    /// <param name="request">Optional assignment filters and paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of assignments for the work item.</returns>
    Task<PagedResult<IAssignment>> GetAssignmentsAsync(Guid id, AssignmentSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the notes associated with a work item.</summary>
    /// <param name="id">The work item identifier.</param>
    /// <param name="request">Optional note filters and paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of notes for the work item.</returns>
    Task<PagedResult<INote>> GetNotesAsync(Guid id, NoteSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the actual time entries associated with a work item.</summary>
    /// <param name="id">The work item identifier.</param>
    /// <param name="request">Optional time-entry filters and paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of time entries for the work item.</returns>
    Task<PagedResult<ITimeEntry>> GetTimeEntriesAsync(Guid id, TimeEntrySearchRequest request, CancellationToken cancellationToken = default);
}
