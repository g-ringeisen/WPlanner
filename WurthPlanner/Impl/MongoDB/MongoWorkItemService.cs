using MongoDB.Bson;
using MongoDB.Driver;
using WurthPlanner.Contracts.Assignments;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Contracts.Notes;
using WurthPlanner.Contracts.TimeEntries;
using WurthPlanner.Contracts.WorkItems;
using WurthPlanner.Models;
using WurthPlanner.Services;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Persists work items and enforces their hierarchy and deletion rules in MongoDB.
/// </summary>
public sealed class MongoWorkItemService : IWorkItemService
{
    private readonly MongoDbContext _context;
    private readonly ICurrentUser _currentUser;

    public MongoWorkItemService(MongoDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IWorkItem> CreateAsync(
        CreateWorkItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title);

        if (request.ParentId is { } parentId)
        {
            await GetRequiredAsync(parentId, cancellationToken);
        }

        ValidateEffort(request.EffortDays);

        var workItem = new WorkItem
        {
            Title = request.Title.Trim(),
            Type = request.Type,
            Status = request.Status ?? WorkItemStatus.Draft,
            Description = MongoMapping.ToTextContent(request.Description),
            Tags = request.Tags ?? Array.Empty<string>(),
            DueDate = request.DueDate,
            EffortDays = request.EffortDays,
            ParentId = request.ParentId,
            Source = MongoMapping.ToExternalReference(request.Source)
        };

        MongoMapping.SetCreatedAudit(workItem, _currentUser.Id);
        await _context.WorkItems.InsertOneAsync(workItem, cancellationToken: cancellationToken);

        return workItem;
    }

    public async Task<IWorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems.Find(item => item.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IWorkItem> UpdateAsync(
        Guid id,
        UpdateWorkItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title);
        ValidateEffort(request.EffortDays);

        if (request.ParentId == id)
        {
            throw new ArgumentException("A work item cannot be its own parent.", nameof(request));
        }

        if (request.ParentId is { } parentId)
        {
            await GetRequiredAsync(parentId, cancellationToken);
            await EnsureNoHierarchyCycleAsync(id, parentId, cancellationToken);
        }

        var workItem = await GetRequiredAsync(id, cancellationToken);
        workItem.Title = request.Title.Trim();
        workItem.Status = request.Status;
        workItem.Type = request.Type;
        workItem.Description = MongoMapping.ToTextContent(request.Description);
        workItem.Tags = request.Tags ?? Array.Empty<string>();
        workItem.DueDate = request.DueDate;
        workItem.EffortDays = request.EffortDays;
        workItem.ParentId = request.ParentId;
        workItem.Source = MongoMapping.ToExternalReference(request.Source);

        MongoMapping.SetUpdatedAudit(workItem, _currentUser.Id);
        await _context.WorkItems.ReplaceOneAsync(
            item => item.Id == id,
            workItem,
            cancellationToken: cancellationToken);

        return workItem;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await GetRequiredAsync(id, cancellationToken);

        var hasChildren = await _context.WorkItems.Find(item => item.ParentId == id).AnyAsync(cancellationToken);
        var hasAssignments = await _context.Assignments.Find(item => item.WorkItemId == id).AnyAsync(cancellationToken);
        var hasTimeEntries = await _context.TimeEntries.Find(item => item.WorkItemId == id).AnyAsync(cancellationToken);
        var hasNotes = await _context.Notes.Find(item => item.WorkItemId == id).AnyAsync(cancellationToken);

        if (hasChildren || hasAssignments || hasTimeEntries || hasNotes)
        {
            throw new InvalidOperationException("The work item has dependent data and cannot be deleted.");
        }

        await _context.WorkItems.DeleteOneAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<PagedResult<IWorkItemBrief>> SearchAsync(
        WorkItemSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildSearchFilter(request);
        var page = MongoPaging.Normalize(request.PageNumber, request.PageSize);
        var total = (int)await _context.WorkItems.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await _context.WorkItems
            .Find(filter)
            .SortByDescending(item => item.UpdatedAt)
            .Skip(page.Skip)
            .Limit(page.Size)
            .ToListAsync(cancellationToken);

        return new PagedResult<IWorkItemBrief>(
            items.Select(MongoMapping.ToBrief).ToArray(),
            page.Number,
            page.Size,
            total);
    }

    public async Task<IWorkItemBrief?> GetParentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await GetByIdAsync(id, cancellationToken);
        var parent = item?.ParentId is { } parentId
            ? await GetByIdAsync(parentId, cancellationToken) as WorkItem
            : null;
        return parent is null ? null : MongoMapping.ToBrief(parent);
    }

    public async Task<IReadOnlyCollection<IWorkItemBrief>> GetAncestorPathAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = new List<WorkItemBrief>();
        var current = await GetByIdAsync(id, cancellationToken) as WorkItem;

        while (current?.ParentId is { } parentId)
        {
            current = await GetByIdAsync(parentId, cancellationToken) as WorkItem;
            if (current is null) break;
            result.Insert(0, MongoMapping.ToBrief(current));
        }

        return result;
    }

    public Task<PagedResult<IWorkItemBrief>> GetChildrenAsync(
        Guid id,
        PageRequest request,
        CancellationToken cancellationToken = default)
    {
        return SearchAsync(
            new WorkItemSearchRequest { ParentId = id, PageNumber = request.PageNumber, PageSize = request.PageSize },
            cancellationToken);
    }

    public Task<PagedResult<IAssignment>> GetAssignmentsAsync(Guid id, AssignmentSearchRequest request, CancellationToken cancellationToken = default) =>
        new MongoAssignmentService(_context, _currentUser).SearchAsync(request with { WorkItemId = id }, cancellationToken);

    public Task<PagedResult<INote>> GetNotesAsync(Guid id, NoteSearchRequest request, CancellationToken cancellationToken = default) =>
        new MongoNoteService(_context, _currentUser).SearchAsync(request with { WorkItemId = id }, cancellationToken);

    public Task<PagedResult<ITimeEntry>> GetTimeEntriesAsync(Guid id, TimeEntrySearchRequest request, CancellationToken cancellationToken = default) =>
        new MongoTimeEntryService(_context, _currentUser).SearchAsync(request with { WorkItemId = id }, cancellationToken);

    private async Task<WorkItem> GetRequiredAsync(Guid id, CancellationToken cancellationToken) =>
        await GetByIdAsync(id, cancellationToken) as WorkItem ?? throw new KeyNotFoundException($"Work item '{id}' was not found.");

    private async Task EnsureNoHierarchyCycleAsync(Guid workItemId, Guid parentId, CancellationToken cancellationToken)
    {
        var currentParentId = parentId;
        while (true)
        {
            if (currentParentId == workItemId)
            {
                throw new ArgumentException("The selected parent would create a hierarchy cycle.");
            }

            var parent = await GetByIdAsync(currentParentId, cancellationToken);
            if (parent?.ParentId is not { } nextParentId) return;
            currentParentId = nextParentId;
        }
    }

    private static FilterDefinition<WorkItem> BuildSearchFilter(WorkItemSearchRequest request)
    {
        var filter = Builders<WorkItem>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var expression = new BsonRegularExpression(request.SearchText.Trim(), "i");
            filter &= Builders<WorkItem>.Filter.Or(
                Builders<WorkItem>.Filter.Regex(item => item.Title, expression),
                Builders<WorkItem>.Filter.Regex("Description.Content", expression));
        }

        if (request.ParentId is { } parentId) filter &= Builders<WorkItem>.Filter.Eq(item => item.ParentId, parentId);
        if (request.HasParent is true) filter &= Builders<WorkItem>.Filter.Ne(item => item.ParentId, null);
        if (request.HasParent is false) filter &= Builders<WorkItem>.Filter.Eq(item => item.ParentId, null);
        if (request.DueFrom is { } dueFrom) filter &= Builders<WorkItem>.Filter.Gte(item => item.DueDate, dueFrom);
        if (request.DueTo is { } dueTo) filter &= Builders<WorkItem>.Filter.Lte(item => item.DueDate, dueTo);

        return filter;
    }

    private static void ValidateEffort(decimal? effortDays)
    {
        if (effortDays is < 0) throw new ArgumentException("EffortDays cannot be negative.");
    }
}
