using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WurthPlanner.Impl.MongoDB;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables(prefix: "WPLANNER_")
    .AddCommandLine(args);

builder.Services.AddWurthPlannerMongoDb(builder.Configuration);

using var host = builder.Build();

var logger = host.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("MongoDbInitializer");

try
{
    logger.LogInformation("Initialisation de la structure MongoDB Würth Planner...");

    await host.Services.InitializeWurthPlannerMongoDbAsync();

    logger.LogInformation("Initialisation terminée. Les collections et index requis sont prêts.");
    return 0;
}
catch (Exception exception)
{
    logger.LogCritical(exception, "L'initialisation MongoDB a échoué.");
    return 1;
}
