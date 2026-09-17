using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Exposes the audit metadata of a read model.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Gets the stable identifier of the user who created the item.
        /// </summary>
        string CreatedBy { get; }

        /// <summary>
        /// Gets the stable identifier of the user who last updated the item.
        /// </summary>
        string UpdatedBy { get; }

        /// <summary>
        /// Gets the date and time when the item was created.
        /// </summary>
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Gets the date and time when the item was last updated.
        /// </summary>
        DateTimeOffset UpdatedAt { get; }
    }
}
