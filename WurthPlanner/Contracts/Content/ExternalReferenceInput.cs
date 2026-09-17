namespace WurthPlanner.Contracts.Content;

public sealed record ExternalReferenceInput(
    string System,
    string Id,
    string? Title = null,
    string? Url = null);
