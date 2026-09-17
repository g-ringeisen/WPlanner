namespace WurthPlanner.Models
{
    /// <summary>
    /// Represents text together with the format used to interpret it.
    /// </summary>
    public interface ITextContent
    {
        /// <summary>
        /// Gets the text content.
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Gets the MIME type or format of the content.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets a short excerpt of the content, when available.
        /// </summary>
        string? Excerpt { get; }
    }
}
