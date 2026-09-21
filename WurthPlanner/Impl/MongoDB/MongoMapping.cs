using System.Reflection;
using WurthPlanner.Contracts.Content;
using WurthPlanner.Models;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Maps application contracts to domain models and applies common audit metadata.
/// </summary>
internal static class MongoMapping
{
    public static TextContent ToTextContent(TextContentInput input) => new()
    {
        Content = input.Content,
        ContentType = input.ContentType,
        Excerpt = CreateExcerpt(input.Content)
    };

    public static ExternalReference? ToExternalReference(ExternalReferenceInput? input) =>
        input is null
            ? null
            : new ExternalReference { System = input.System, Id = input.Id, Title = input.Title, Url = input.Url };

    public static WorkItemBrief ToBrief(WorkItem item)
    {
        var brief = new WorkItemBrief
        {
            Title = item.Title,
            Status = item.Status,
            Type = item.Type,
            Excerpt = item.Excerpt,
            Source = item.Source
        };

        // WorkItemBrief exposes its identifier with a private setter in the domain project.
        var property = typeof(WorkItemBrief).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        property.SetValue(brief, item.Id);
        return brief;
    }

    public static void SetCreatedAudit(Auditable entity, string userId)
    {
        var now = DateTimeOffset.UtcNow;
        entity.CreatedAt = now;
        entity.UpdatedAt = now;
        entity.CreatedBy = userId;
        entity.UpdatedBy = userId;
    }

    public static void SetUpdatedAudit(Auditable entity, string userId)
    {
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedBy = userId;
    }

    private static string? CreateExcerpt(string? content) =>
        string.IsNullOrWhiteSpace(content) ? null : content.Length <= 240 ? content : content[..240];
}

/// <summary>
/// Provides bounded paging values shared by MongoDB queries.
/// </summary>
internal static class MongoPaging
{
    public static (int Number, int Size, int Skip) Normalize(int pageNumber, int pageSize)
    {
        var number = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 500);
        return (number, size, (number - 1) * size);
    }
}
