namespace WurthPlanner.Contracts.Content;

/// <summary>Text submitted by a caller. The content type may be text/plain or text/markdown.</summary>
public sealed record TextContentInput(string Content, string ContentType = "text/plain");
