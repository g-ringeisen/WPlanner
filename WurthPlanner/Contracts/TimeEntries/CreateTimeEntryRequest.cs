using System;
using WurthPlanner.Contracts.Content;

namespace WurthPlanner.Contracts.TimeEntries;

public sealed record CreateTimeEntryRequest(
    Guid WorkItemId,
    string EmployeeId,
    DateOnly WorkDate,
    decimal ActualHours,
    Guid? AssignmentId = null,
    TextContentInput? Comment = null);
