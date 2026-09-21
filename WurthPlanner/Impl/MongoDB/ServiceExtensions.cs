using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WurthPlanner.Services;

namespace WurthPlanner.Impl.MongoDB;

/// <summary>
/// Registers MongoDB persistence and the Würth Planner service implementations.
/// </summary>
public static class ServiceExtensions
{
    public static IServiceCollection AddWurthPlannerMongoDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<MongoDbSettings>()
            .Bind(configuration.GetSection(MongoDbSettings.SectionName))
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.ConnectionString),
                "MongoDb:ConnectionString is required.")
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.DatabaseName),
                "MongoDb:DatabaseName is required.")
            .ValidateOnStart();

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<ICurrentUser, SystemCurrentUser>();
        services.AddSingleton<IMongoDbInitializer, MongoDbInitializer>();

        services.AddScoped<IWorkItemService, MongoWorkItemService>();
        services.AddScoped<IAssignmentService, MongoAssignmentService>();
        services.AddScoped<ITimeEntryService, MongoTimeEntryService>();
        services.AddScoped<INoteService, MongoNoteService>();
        services.AddScoped<IPlanningService, MongoPlanningService>();

        return services;
    }

    public static Task InitializeWurthPlannerMongoDbAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        return serviceProvider
            .GetRequiredService<IMongoDbInitializer>()
            .InitializeAsync(cancellationToken);
    }
}
