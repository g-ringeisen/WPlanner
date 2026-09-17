namespace WurthPlanner.Models
{
    /// <summary>
    /// Instantiable model for a reference managed by an external system.
    /// </summary>
    public class ExternalReference : IExternalReference
    {
        public string System { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Url { get; set; }
    }
}
