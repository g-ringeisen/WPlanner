using MongoDB.Driver;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Contracts.TimeEntries;
using WurthPlanner.Models;
using WurthPlanner.Services;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Stores actual time entries and validates their optional relationship with assignments.
/// </summary>
public sealed class MongoTimeEntryService : ITimeEntryService
{
    private readonly MongoDbContext _context;
    private readonly ICurrentUser _currentUser;

    public MongoTimeEntryService(MongoDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ITimeEntry> CreateAsync(CreateTimeEntryRequest request, CancellationToken cancellationToken = default)
    {
        var entry = new TimeEntry
        {
            WorkItemId = request.WorkItemId,
            EmployeeId = request.EmployeeId,
            WorkDate = request.WorkDate,
            ActualHours = request.ActualHours,
            AssignmentId = request.AssignmentId,
            Comment = request.Comment is null ? null : MongoMapping.ToTextContent(request.Comment)
        };

        await ValidateAsync(entry, cancellationToken);
        MongoMapping.SetCreatedAudit(entry, _currentUser.Id);
        await _context.TimeEntries.InsertOneAsync(entry, cancellationToken: cancellationToken);
        return entry;
    }

    public async Task<ITimeEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.TimeEntries.Find(entry => entry.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<ITimeEntry> UpdateAsync(Guid id, UpdateTimeEntryRequest request, CancellationToken cancellationToken = default)
    {
        var entry = await GetRequiredAsync(id, cancellationToken);
        entry.EmployeeId = request.EmployeeId;
        entry.WorkDate = request.WorkDate;
        entry.ActualHours = request.ActualHours;
        entry.AssignmentId = request.AssignmentId;
        entry.Comment = request.Comment is null ? null : MongoMapping.ToTextContent(request.Comment);

        await ValidateAsync(entry, cancellationToken);
        MongoMapping.SetUpdatedAudit(entry, _currentUser.Id);
        await _context.TimeEntries.ReplaceOneAsync(item => item.Id == id, entry, cancellationToken: cancellationToken);
        return entry;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _context.TimeEntries.DeleteOneAsync(entry => entry.Id == id, cancellationToken);
        if (result.DeletedCount == 0) throw new KeyNotFoundException($"Time entry '{id}' was not found.");
    }

    public async Task<PagedResult<ITimeEntry>> SearchAsync(TimeEntrySearchRequest request, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TimeEntry>.Filter.Empty;
        if (request.WorkItemId is { } workItemId) filter &= Builders<TimeEntry>.Filter.Eq(item => item.WorkItemId, workItemId);
        if (request.AssignmentId is { } assignmentId) filter &= Builders<TimeEntry>.Filter.Eq(item => item.AssignmentId, assignmentId);
        if (!string.IsNullOrWhiteSpace(request.EmployeeId)) filter &= Builders<TimeEntry>.Filter.Eq(item => item.EmployeeId, request.EmployeeId);
        if (request.WorkDateFrom is { } from) filter &= Builders<TimeEntry>.Filter.Gte(item => item.WorkDate, from);
        if (request.WorkDateTo is { } to) filter &= Builders<TimeEntry>.Filter.Lte(item => item.WorkDate, to);

        var page = MongoPaging.Normalize(request.PageNumber, request.PageSize);
        var total = (int)await _context.TimeEntries.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await _context.TimeEntries.Find(filter).SortByDescending(item => item.WorkDate).Skip(page.Skip).Limit(page.Size).ToListAsync(cancellationToken);
        return new PagedResult<ITimeEntry>([.. items.Cast<ITimeEntry>()], page.Number, page.Size, total);
    }

    public async Task<IWorkItemBrief?> GetWorkItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entry = await GetByIdAsync(id, cancellationToken);
        var workItem = entry is null ? null : await _context.WorkItems.Find(item => item.Id == entry.WorkItemId).FirstOrDefaultAsync(cancellationToken);
        return workItem is null ? null : MongoMapping.ToBrief(workItem);
    }

    public async Task<IAssignment?> GetAssignmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entry = await GetByIdAsync(id, cancellationToken);
        return entry?.AssignmentId is { } assignmentId
            ? await _context.Assignments.Find(item => item.Id == assignmentId).FirstOrDefaultAsync(cancellationToken)
            : null;
    }

    private async Task<TimeEntry> GetRequiredAsync(Guid id, CancellationToken cancellationToken) =>
        await GetByIdAsync(id, cancellationToken) as TimeEntry ?? throw new KeyNotFoundException($"Time entry '{id}' was not found.");

    private async Task ValidateAsync(TimeEntry entry, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entry.EmployeeId);
        if (entry.ActualHours <= 0) throw new ArgumentException("ActualHours must be strictly positive.");
        if (await _context.WorkItems.Find(item => item.Id == entry.WorkItemId).FirstOrDefaultAsync(cancellationToken) is null) throw new KeyNotFoundException("The work item does not exist.");
        if (entry.AssignmentId is not { } assignmentId) return;

        var assignment = await _context.Assignments.Find(item => item.Id == assignmentId).FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("The assignment does not exist.");
        if (assignment.WorkItemId != entry.WorkItemId) throw new ArgumentException("The assignment must belong to the same work item.");
    }
}
