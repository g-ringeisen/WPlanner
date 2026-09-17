using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Instantiable base model containing audit metadata.
    /// </summary>
    public class Auditable : IAuditable
    {
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
