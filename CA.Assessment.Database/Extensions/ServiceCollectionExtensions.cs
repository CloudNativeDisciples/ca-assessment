using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Store.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssessmentDatabase(
        this IServiceCollection serviceCollection,
        string connectionString)
    {
        if (serviceCollection is null) throw new ArgumentNullException(nameof(serviceCollection));
        if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));

        serviceCollection.AddSingleton(new DatabaseConnectionString(connectionString));

        serviceCollection.AddScoped<DatabaseSessionManager>();

        serviceCollection.AddTransient<IDatabaseSession>(sc => sc.GetRequiredService<DatabaseSessionManager>());
        serviceCollection.AddTransient<IDatabaseSessionManager>(sc => sc.GetRequiredService<DatabaseSessionManager>());

        return serviceCollection;
    }
}
