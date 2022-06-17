using CA.Assessment.Images.Stores.FileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Images.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileSystemImageStore(this IServiceCollection serviceCollection,
        Action<FileSystemImageStoreOptions> configure)
    {
        if (serviceCollection is null)
        {
            throw new ArgumentNullException(nameof(serviceCollection));
        }

        serviceCollection.AddOptions<FileSystemImageStoreOptions>()
            .Configure(configure);

        serviceCollection.AddTransient<IImageStore, FileSystemImageStore>();

        return serviceCollection;
    }
}