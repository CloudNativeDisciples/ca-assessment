using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Database.Migrations.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssessmentMigrations(
        this IServiceCollection serviceCollection,
        string connectionString)
    {
        if (serviceCollection is null) throw new ArgumentNullException(nameof(serviceCollection));
        if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));

        serviceCollection.AddFluentMigratorCore()
            .ConfigureRunner(runner => runner.AddSQLite()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(ServiceCollectionExtensions).Assembly).For.Migrations()
            );

        return serviceCollection;
    }
}
