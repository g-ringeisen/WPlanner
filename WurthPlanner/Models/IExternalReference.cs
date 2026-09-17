namespace WurthPlanner.Models
{
    /// <summary>
    /// Represents a reference to an item managed by an external system.
    /// </summary>
    public interface IExternalReference
    {
        /// <summary>
        /// Gets the name of the external system that owns the referenced item.
        /// </summary>
        string System { get; }

        /// <summary>
        /// Gets the identifier of the referenced item in the external system.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the display title of the referenced item, when available.
        /// </summary>
        string? Title { get; }

        /// <summary>
        /// Gets the URL of the referenced item, when available.
        /// </summary>
        string? Url { get; }
    }
}
