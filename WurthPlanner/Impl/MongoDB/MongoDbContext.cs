using MongoDB.Driver;
using WurthPlanner.Models;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Centralizes access to the MongoDB collections used by Würth Planner.
/// </summary>
public sealed class MongoDbContext
{
    public MongoDbContext(IMongoDatabase database)
    {
        WorkItems = database.GetCollection<WorkItem>("workItems");
        Assignments = database.GetCollection<Assignment>("assignments");
        TimeEntries = database.GetCollection<TimeEntry>("timeEntries");
        Notes = database.GetCollection<Note>("notes");
    }

    public IMongoCollection<WorkItem> WorkItems { get; }

    public IMongoCollection<Assignment> Assignments { get; }

    public IMongoCollection<TimeEntry> TimeEntries { get; }

    public IMongoCollection<Note> Notes { get; }
}
