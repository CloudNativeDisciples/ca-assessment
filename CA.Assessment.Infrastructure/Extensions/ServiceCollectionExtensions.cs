using CA.Assessment.Application.Factories;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Infrastructure.Factories;
using CA.Assessment.Infrastructure.Images;
using CA.Assessment.Infrastructure.Mappers;
using CA.Assessment.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssessmentInfrastructure(
        this IServiceCollection serviceCollection,
        string imagesStoreFolder)
    {
        if (serviceCollection is null) throw new ArgumentNullException(nameof(serviceCollection));
        if (imagesStoreFolder is null) throw new ArgumentNullException(nameof(imagesStoreFolder));

        serviceCollection.AddSingleton(new ImageStoreFolder(imagesStoreFolder));

        serviceCollection.AddTransient<ImageStore>();

        serviceCollection.AddTransient<IBlogPostRepository, SQLiteBlogPostRepository>();
        serviceCollection.AddTransient<ITagsRepository, SQLiteTagsRepository>();
        serviceCollection.AddTransient<ICategoryRepository, SQLiteCategoriesRepository>();
        serviceCollection.AddTransient<IImagesRepository, SQLiteImagesRepository>();

        serviceCollection.AddTransient<TagRowsMapper>();
        serviceCollection.AddTransient<CategoryRowsMapper>();
        serviceCollection.AddTransient<BlogPostRowsMapper>();
        serviceCollection.AddTransient<BlogPostSummaryRowMapper>();
        serviceCollection.AddTransient<ImageRowsMapper>();

        serviceCollection.AddTransient<IDatabaseConnectionFactory, SQLiteDatabaseConnectionFactory>();

        return serviceCollection;
    }
}
