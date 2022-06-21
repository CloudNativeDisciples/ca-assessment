using CA.Assessment.Application.Factories;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Database.Sqlite.Factories;
using CA.Assessment.Database.Sqlite.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Database.Sqlite.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssessmentSqliteDatabase(this IServiceCollection serviceCollection)
    {
        if (serviceCollection is null)
        {
            throw new ArgumentNullException(nameof(serviceCollection));
        }

        serviceCollection.AddTransient<IDatabaseConnectionFactory, SqliteDatabaseConnectionFactory>();

        serviceCollection.AddTransient<IBlogPostRepository, SqliteBlogPostRepository>();
        serviceCollection.AddTransient<ITagsRepository, SqliteTagsRepository>();
        serviceCollection.AddTransient<ICategoryRepository, SqliteCategoriesRepository>();
        serviceCollection.AddTransient<IImagesRepository, SqliteImagesRepository>();

        return serviceCollection;
    }
}
