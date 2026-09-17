using System;
using WurthPlanner.Contracts.Content;

namespace WurthPlanner.Contracts.Notes;

public sealed record CreateNoteRequest(Guid WorkItemId, string Title, TextContentInput Content);
