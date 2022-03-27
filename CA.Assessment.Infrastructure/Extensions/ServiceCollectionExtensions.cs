using CA.Assessment.Application.Factories;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Infrastructure.Factories;
using CA.Assessment.Infrastructure.Mappers;
using CA.Assessment.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssessmentInfrastructure(this IServiceCollection serviceCollection)
    {
        if (serviceCollection is null) throw new ArgumentNullException(nameof(serviceCollection));

        serviceCollection.AddTransient<IBlogPostRepository, SQLiteBlogPostRepository>();
        serviceCollection.AddTransient<ITagsRepository, SQLiteTagsRepository>();
        serviceCollection.AddTransient<ICategoryRepository, SQLiteCategoriesRepository>();

        serviceCollection.AddTransient<TagRowsMapper>();
        serviceCollection.AddTransient<CategoryRowsMapper>();
        serviceCollection.AddTransient<BlogPostRowsMapper>();

        serviceCollection.AddTransient<IDatabaseConnectionFactory, SQLiteDatabaseConnectionFactory>();

        return serviceCollection;
    }
}
