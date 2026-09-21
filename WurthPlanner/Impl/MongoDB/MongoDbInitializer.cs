using MongoDB.Driver;
using WurthPlanner.Models;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Creates the indexes required by the MongoDB queries at application startup.
/// The operation is idempotent and can safely run more than once.
/// </summary>
public interface IMongoDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed class MongoDbInitializer : IMongoDbInitializer
{
    private readonly MongoDbContext _context;

    public MongoDbInitializer(MongoDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _context.WorkItems.Indexes.CreateManyAsync(
            [
                new CreateIndexModel<WorkItem>(
                    Builders<WorkItem>.IndexKeys.Ascending(item => item.ParentId)),
                new CreateIndexModel<WorkItem>(
                    Builders<WorkItem>.IndexKeys.Ascending(item => item.DueDate)),
                new CreateIndexModel<WorkItem>(
                    Builders<WorkItem>.IndexKeys
                        .Ascending(item => item.Status)
                        .Ascending(item => item.Type)),
                new CreateIndexModel<WorkItem>(
                    Builders<WorkItem>.IndexKeys.Descending(item => item.UpdatedAt))
            ],
            cancellationToken: cancellationToken);

        await _context.Assignments.Indexes.CreateManyAsync(
            [
                new CreateIndexModel<Assignment>(
                    Builders<Assignment>.IndexKeys.Ascending(assignment => assignment.WorkItemId)),
                new CreateIndexModel<Assignment>(
                    Builders<Assignment>.IndexKeys
                        .Ascending(assignment => assignment.AssigneeId)
                        .Ascending(assignment => assignment.StartDate)
                        .Ascending(assignment => assignment.EndDate))
            ],
            cancellationToken: cancellationToken);

        await _context.TimeEntries.Indexes.CreateManyAsync(
            [
                new CreateIndexModel<TimeEntry>(
                    Builders<TimeEntry>.IndexKeys
                        .Ascending(entry => entry.WorkItemId)
                        .Ascending(entry => entry.WorkDate)),
                new CreateIndexModel<TimeEntry>(
                    Builders<TimeEntry>.IndexKeys.Ascending(entry => entry.AssignmentId)),
                new CreateIndexModel<TimeEntry>(
                    Builders<TimeEntry>.IndexKeys
                        .Ascending(entry => entry.EmployeeId)
                        .Ascending(entry => entry.WorkDate))
            ],
            cancellationToken: cancellationToken);

        await _context.Notes.Indexes.CreateOneAsync(
            new CreateIndexModel<Note>(
                Builders<Note>.IndexKeys
                    .Ascending(note => note.WorkItemId)
                    .Ascending(note => note.CreatedAt)),
            cancellationToken: cancellationToken);
    }
}
