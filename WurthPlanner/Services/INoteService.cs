using System;
using System.Threading;
using System.Threading.Tasks;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Contracts.Notes;
using WurthPlanner.Models;

namespace WurthPlanner.Services;

/// <summary>Provides application operations for the chronological notes associated with work items.</summary>
public interface INoteService
{
    /// <summary>Creates a note associated with a work item.</summary>
    /// <param name="request">The target work item, title and formatted note content.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The newly created note.</returns>
    Task<INote> CreateAsync(CreateNoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a note by its identifier.</summary>
    /// <param name="id">The note identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The note, or <see langword="null"/> when it does not exist.</returns>
    Task<INote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Updates a note title and content.</summary>
    /// <param name="id">The note identifier.</param>
    /// <param name="request">The replacement values for the note.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The updated note.</returns>
    Task<INote> UpdateAsync(Guid id, UpdateNoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a note.</summary>
    /// <param name="id">The note identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Searches notes by work item, free text, sorting and pagination.</summary>
    /// <param name="request">The search criteria and paging options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A page of matching notes.</returns>
    Task<PagedResult<INote>> SearchAsync(NoteSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the lightweight work item associated with a note.</summary>
    /// <param name="id">The note identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The related work item, or <see langword="null"/> when the note does not exist.</returns>
    Task<IWorkItemBrief?> GetWorkItemAsync(Guid id, CancellationToken cancellationToken = default);
}
