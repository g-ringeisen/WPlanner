using WurthPlanner.Contracts.Content;

namespace WurthPlanner.Contracts.Notes;

public sealed record UpdateNoteRequest(string Title, TextContentInput Content);
