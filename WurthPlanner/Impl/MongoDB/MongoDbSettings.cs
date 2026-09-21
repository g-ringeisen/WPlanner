namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Holds the MongoDB connection parameters for Würth Planner.
/// </summary>
public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; init; } = "mongodb://localhost:27017";

    public string DatabaseName { get; init; } = "wurth-planner";
}

/// <summary>
/// Supplies the identity used to populate audit fields.
/// Applications should replace the default implementation with one backed by authentication.
/// </summary>
public interface ICurrentUser
{
    string Id { get; }
}

/// <summary>
/// Default technical identity used when no authentication provider is registered.
/// </summary>
public sealed class SystemCurrentUser : ICurrentUser
{
    public string Id => "system";
}
