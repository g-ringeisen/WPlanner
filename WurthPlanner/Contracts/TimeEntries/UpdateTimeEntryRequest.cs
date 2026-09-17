using System;
using WurthPlanner.Contracts.Content;

namespace WurthPlanner.Contracts.TimeEntries;

public sealed record UpdateTimeEntryRequest(
    string EmployeeId,
    DateOnly WorkDate,
    decimal ActualHours,
    Guid? AssignmentId,
    TextContentInput? Comment);
