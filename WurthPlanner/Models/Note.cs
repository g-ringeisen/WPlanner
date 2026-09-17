using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Instantiable note model associated with a work item.
    /// </summary>
    public class Note : Auditable, INote
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid WorkItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public ITextContent Content { get; set; } = new TextContent();
        public string? Excerpt => Content.Excerpt;
    }
}
