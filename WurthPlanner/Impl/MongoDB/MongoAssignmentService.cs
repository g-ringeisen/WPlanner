using MongoDB.Driver;
using WurthPlanner.Contracts.Assignments;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Contracts.TimeEntries;
using WurthPlanner.Models;
using WurthPlanner.Services;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Manages planned work allocations independently from recorded actual time.
/// </summary>
public sealed class MongoAssignmentService : IAssignmentService
{
    private readonly MongoDbContext _context;
    private readonly ICurrentUser _currentUser;

    public MongoAssignmentService(MongoDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IAssignment> CreateAsync(CreateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureWorkItemExistsAsync(request.WorkItemId, cancellationToken);
        ValidatePlanning(request.StartDate, request.EndDate, request.PlannedDays);

        var assignment = new Assignment
        {
            WorkItemId = request.WorkItemId,
            AssigneeId = request.AssigneeId,
            Instruction = MongoMapping.ToTextContent(request.Instruction),
            PlannedDays = request.PlannedDays,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        MongoMapping.SetCreatedAudit(assignment, _currentUser.Id);
        await _context.Assignments.InsertOneAsync(assignment, cancellationToken: cancellationToken);
        return assignment;
    }

    public async Task<IAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Assignments.Find(assignment => assignment.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<IAssignment> UpdateAsync(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        ValidatePlanning(request.StartDate, request.EndDate, request.PlannedDays);
        var assignment = await GetRequiredAsync(id, cancellationToken);

        assignment.AssigneeId = request.AssigneeId;
        assignment.Instruction = MongoMapping.ToTextContent(request.Instruction);
        assignment.PlannedDays = request.PlannedDays;
        assignment.StartDate = request.StartDate;
        assignment.EndDate = request.EndDate;

        MongoMapping.SetUpdatedAudit(assignment, _currentUser.Id);
        await _context.Assignments.ReplaceOneAsync(x => x.Id == id, assignment, cancellationToken: cancellationToken);
        return assignment;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await GetRequiredAsync(id, cancellationToken);
        if (await _context.TimeEntries.Find(entry => entry.AssignmentId == id).AnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("The assignment has linked time entries and cannot be deleted.");
        }

        await _context.Assignments.DeleteOneAsync(assignment => assignment.Id == id, cancellationToken);
    }

    public async Task<PagedResult<IAssignment>> SearchAsync(AssignmentSearchRequest request, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Assignment>.Filter.Empty;
        if (request.WorkItemId is { } workItemId) filter &= Builders<Assignment>.Filter.Eq(item => item.WorkItemId, workItemId);
        if (!string.IsNullOrWhiteSpace(request.AssigneeId)) filter &= Builders<Assignment>.Filter.Eq(item => item.AssigneeId, request.AssigneeId);
        if (request.OverlapFrom is { } from) filter &= Builders<Assignment>.Filter.Or(Builders<Assignment>.Filter.Eq(item => item.EndDate, null), Builders<Assignment>.Filter.Gte(item => item.EndDate, from));
        if (request.OverlapTo is { } to) filter &= Builders<Assignment>.Filter.Or(Builders<Assignment>.Filter.Eq(item => item.StartDate, null), Builders<Assignment>.Filter.Lte(item => item.StartDate, to));

        var page = MongoPaging.Normalize(request.PageNumber, request.PageSize);
        var total = (int)await _context.Assignments.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await _context.Assignments.Find(filter).SortBy(item => item.StartDate).Skip(page.Skip).Limit(page.Size).ToListAsync(cancellationToken);
        return new PagedResult<IAssignment>([.. items.Cast<IAssignment>()], page.Number, page.Size, total);
    }

    public async Task<IWorkItemBrief?> GetWorkItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetByIdAsync(id, cancellationToken);
        var workItem = assignment is null ? null : await _context.WorkItems.Find(item => item.Id == assignment.WorkItemId).FirstOrDefaultAsync(cancellationToken);
        return workItem is null ? null : MongoMapping.ToBrief(workItem);
    }

    public Task<PagedResult<ITimeEntry>> GetTimeEntriesAsync(Guid id, TimeEntrySearchRequest request, CancellationToken cancellationToken = default) =>
        new MongoTimeEntryService(_context, _currentUser).SearchAsync(request with { AssignmentId = id }, cancellationToken);

    private async Task<Assignment> GetRequiredAsync(Guid id, CancellationToken cancellationToken) =>
        await GetByIdAsync(id, cancellationToken) as Assignment ?? throw new KeyNotFoundException($"Assignment '{id}' was not found.");

    private async Task EnsureWorkItemExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await _context.WorkItems.Find(item => item.Id == id).FirstOrDefaultAsync(cancellationToken) is null)
            throw new KeyNotFoundException($"Work item '{id}' was not found.");
    }

    private static void ValidatePlanning(DateOnly? startDate, DateOnly? endDate, decimal? plannedDays)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate) throw new ArgumentException("StartDate must not be after EndDate.");
        if (plannedDays is <= 0) throw new ArgumentException("PlannedDays must be positive when specified.");
    }
}
