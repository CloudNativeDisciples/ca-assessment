using CA.Assessment.Application.Factories;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Database.Sqlite.Factories;
using CA.Assessment.Database.Sqlite.Mappers;
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

        serviceCollection.AddTransient<IBlogPostRepository, SqliteBlogPostRepository>();
        serviceCollection.AddTransient<ITagsRepository, SqliteTagsRepository>();
        serviceCollection.AddTransient<ICategoryRepository, SqliteCategoriesRepository>();
        serviceCollection.AddTransient<IImagesRepository, SqliteImagesRepository>();

        serviceCollection.AddTransient<TagRowsMapper>();
        serviceCollection.AddTransient<CategoryRowsMapper>();
        serviceCollection.AddTransient<BlogPostRowsMapper>();
        serviceCollection.AddTransient<BlogPostSummaryRowMapper>();
        serviceCollection.AddTransient<ImageRowsMapper>();

        serviceCollection.AddTransient<IDatabaseConnectionFactory, SqLiteDatabaseConnectionFactory>();

        return serviceCollection;
    }
}