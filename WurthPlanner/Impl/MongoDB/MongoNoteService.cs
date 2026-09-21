using MongoDB.Bson;
using MongoDB.Driver;
using WurthPlanner.Contracts.Common;
using WurthPlanner.Contracts.Notes;
using WurthPlanner.Models;
using WurthPlanner.Services;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Persists the chronological collaboration notes attached to work items.
/// </summary>
public sealed class MongoNoteService : INoteService
{
    private readonly MongoDbContext _context;
    private readonly ICurrentUser _currentUser;

    public MongoNoteService(MongoDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<INote> CreateAsync(CreateNoteRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title);
        if (await _context.WorkItems.Find(item => item.Id == request.WorkItemId).FirstOrDefaultAsync(cancellationToken) is null)
            throw new KeyNotFoundException("The work item does not exist.");

        var note = new Note { WorkItemId = request.WorkItemId, Title = request.Title.Trim(), Content = MongoMapping.ToTextContent(request.Content) };
        MongoMapping.SetCreatedAudit(note, _currentUser.Id);
        await _context.Notes.InsertOneAsync(note, cancellationToken: cancellationToken);
        return note;
    }

    public async Task<INote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Notes.Find(note => note.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<INote> UpdateAsync(Guid id, UpdateNoteRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title);
        var note = await GetByIdAsync(id, cancellationToken) as Note ?? throw new KeyNotFoundException($"Note '{id}' was not found.");
        note.Title = request.Title.Trim();
        note.Content = MongoMapping.ToTextContent(request.Content);
        MongoMapping.SetUpdatedAudit(note, _currentUser.Id);
        await _context.Notes.ReplaceOneAsync(item => item.Id == id, note, cancellationToken: cancellationToken);
        return note;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _context.Notes.DeleteOneAsync(note => note.Id == id, cancellationToken);
        if (result.DeletedCount == 0) throw new KeyNotFoundException($"Note '{id}' was not found.");
    }

    public async Task<PagedResult<INote>> SearchAsync(NoteSearchRequest request, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Note>.Filter.Empty;
        if (request.WorkItemId is { } workItemId) filter &= Builders<Note>.Filter.Eq(note => note.WorkItemId, workItemId);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var query = new BsonRegularExpression(request.SearchText.Trim(), "i");
            filter &= Builders<Note>.Filter.Or(Builders<Note>.Filter.Regex(note => note.Title, query), Builders<Note>.Filter.Regex("Content.Content", query));
        }

        var page = MongoPaging.Normalize(request.PageNumber, request.PageSize);
        var total = (int)await _context.Notes.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await _context.Notes.Find(filter).SortBy(note => note.CreatedAt).Skip(page.Skip).Limit(page.Size).ToListAsync(cancellationToken);
        return new PagedResult<INote>([.. items.Cast<INote>()], page.Number, page.Size, total);
    }

    public async Task<IWorkItemBrief?> GetWorkItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var note = await GetByIdAsync(id, cancellationToken);
        var workItem = note is null ? null : await _context.WorkItems.Find(item => item.Id == note.WorkItemId).FirstOrDefaultAsync(cancellationToken);
        return workItem is null ? null : MongoMapping.ToBrief(workItem);
    }
}
