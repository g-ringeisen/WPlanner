using System.Reflection;
using WurthPlanner.Contracts.Content;
using WurthPlanner.Models;

namespace WurthPlanner.Impl.MongoDB;

internal static class Mapping
{
    internal static TextContent Content(TextContentInput input) => new() { Content = input.Content, ContentType = input.ContentType, Excerpt = Excerpt(input.Content) };
    internal static ExternalReference? Reference(ExternalReferenceInput? input) => input is null ? null : new() { System = input.System, Id = input.Id, Title = input.Title, Url = input.Url };
    internal static WorkItemBrief Brief(WorkItem x) => new WorkItemBrief() { Title = x.Title, Status = x.Status, Type = x.Type, Excerpt = x.Excerpt, Source = x.Source } .WithId(x.Id);
    internal static string? Excerpt(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Length <= 240 ? text : text[..240];
    internal static T WithId<T>(this T value, Guid id) where T : class
    {
        var property = value.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property!.SetValue(value, id);
        return value;
    }
    internal static void StampCreate(Auditable x, string user) { var now = DateTimeOffset.UtcNow; x.CreatedAt = now; x.UpdatedAt = now; x.CreatedBy = user; x.UpdatedBy = user; }
    internal static void StampUpdate(Auditable x, string user) { x.UpdatedAt = DateTimeOffset.UtcNow; x.UpdatedBy = user; }
    internal static void Require(bool value, string message) { if (!value) throw new ArgumentException(message); }
}
