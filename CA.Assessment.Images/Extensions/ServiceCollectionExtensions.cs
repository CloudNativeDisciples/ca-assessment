using CA.Assessment.Application.Services;
using CA.Assessment.Images.Stores.FileSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Images.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssessmentFileSystemImageStore(this IServiceCollection serviceCollection,
        Action<FileSystemImagesContentStoreOptions> configure)
    {
        if (serviceCollection is null)
        {
            throw new ArgumentNullException(nameof(serviceCollection));
        }

        serviceCollection.AddOptions<FileSystemImagesContentStoreOptions>()
            .Configure(configure);

        serviceCollection.AddTransient<IImagesContentStore, FileSystemImagesContentStore>();

        return serviceCollection;
    }

    public static IServiceCollection AddAssessmentFileSystemImageStore(this IServiceCollection serviceCollection,
        IConfiguration configurationSection)
    {
        if (serviceCollection is null)
        {
            throw new ArgumentNullException(nameof(serviceCollection));
        }

        if (configurationSection is null)
        {
            throw new ArgumentNullException(nameof(configurationSection));
        }

        serviceCollection.AddOptions<FileSystemImagesContentStoreOptions>()
            .Bind(configurationSection);

        serviceCollection.AddTransient<IImagesContentStore, FileSystemImagesContentStore>();

        return serviceCollection;
    }
}
