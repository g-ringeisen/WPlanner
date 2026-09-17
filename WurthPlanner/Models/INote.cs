using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Read model representing a note associated with a work item.
    /// </summary>
    public interface INote : IAuditable
    {
        /// <summary>
        /// Gets the unique identifier of the note.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the identifier of the related work item.
        /// </summary>
        Guid WorkItemId { get; }

        /// <summary>
        /// Gets the title of the note.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the content of the note.
        /// </summary>
        ITextContent Content { get; }

        /// <summary>
        /// Gets a short excerpt of the note content.
        /// </summary>
        string? Excerpt => Content.Excerpt;
    }
}
