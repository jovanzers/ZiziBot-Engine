using Kot.MongoDB.Migrations;
using Kot.MongoDB.Migrations.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ZiziBot.Kot.MongoMigrations;

public static class ServiceExtension
{
    public static IServiceCollection AddKotMongoMigrations(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var connectionStr = EnvUtil.GetEnv(Env.MONGODB_CONNECTION_STRING);
        var url = new MongoUrl(connectionStr);

        var options = new MigrationOptions(url.DatabaseName);

        services.AddMongoMigrations(connectionStr, options, config => config.LoadMigrationsFromCurrentDomain());
        return services;
    }

    public static async Task<IApplicationBuilder> RunMigrationsAsync(this IApplicationBuilder app)
    {
        var serviceProvider = app.ApplicationServices;
        var migrator = serviceProvider.GetRequiredService<IMigrator>();
        var logger = serviceProvider.GetRequiredService<ILogger<IMigrator>>();
        var migrationResult = await migrator.MigrateAsync();

        logger.LogInformation("MongoDB Migration finish. In: {Time}", (migrationResult.FinishTime - migrationResult.StartTime).Duration());

        return app;
    }
}