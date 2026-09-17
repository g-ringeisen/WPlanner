namespace WurthPlanner.Models
{
    /// <summary>
    /// Instantiable text content model.
    /// </summary>
    public class TextContent : ITextContent
    {
        public string Content { get; set; } = string.Empty;
        public string ContentType { get; set; } = "text/plain";
        public string? Excerpt { get; set; }
    }
}
