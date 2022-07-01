using System.Globalization;
using CA.Assessment.Application.Services;
using Microsoft.Extensions.Options;

namespace CA.Assessment.Images.Stores.FileSystem;

public sealed class FileSystemImagesContentStore : IImagesContentStore
{
    private readonly IOptions<FileSystemImagesContentStoreOptions> _config;

    public FileSystemImagesContentStore(IOptions<FileSystemImagesContentStoreOptions> config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public Task<IImageContent?> GetContentAsync(Guid imageRef)
    {
        EnsureImagesFolder();

        var imageFileInfo = GetImageFileInfo(imageRef);

        IImageContent? maybeImageFile = null;

        if (imageFileInfo.Exists)
        {
            maybeImageFile = new ImageFromFileSystemStore(imageRef, imageFileInfo.FullName);
        }

        return Task.FromResult(maybeImageFile);
    }

    public async Task SaveContentAsync(Guid imageRef, Stream imageDataStream)
    {
        if (imageDataStream is null)
        {
            throw new ArgumentNullException(nameof(imageDataStream));
        }

        EnsureImagesFolder();

        var imageFileInfo = GetImageFileInfo(imageRef);

        if (imageFileInfo.Exists && _config.Value.Overwrite)
        {
            imageFileInfo.Delete();
        }

        await using var fileStream = new FileStream(imageFileInfo.FullName, FileMode.CreateNew, FileAccess.Write,
            FileShare.None, 4096, FileOptions.Asynchronous);

        await imageDataStream.CopyToAsync(fileStream);
    }

    private FileInfo GetImageFileInfo(Guid imageRef)
    {
        if (_config.Value.Folder is null)
        {
            throw new DirectoryNotFoundException($"No images directory for {nameof(FileSystemImagesContentStore)} specified");
        }
        
        var imageFileName = GenerateImageFileName(imageRef);

        var imageFullPath = Path.Combine(_config.Value.Folder, imageFileName);

        return new FileInfo(imageFullPath);
    }

    private void EnsureImagesFolder()
    {
        if (_config.Value.Folder is null)
        {
            throw new DirectoryNotFoundException($"No images directory for {nameof(FileSystemImagesContentStore)} specified");
        }
        
        var directoryInfo = new DirectoryInfo(_config.Value.Folder);

        if (directoryInfo.Exists)
        {
            return;
        }

        directoryInfo.Create();
    }

    private static string GenerateImageFileName(Guid imageRef)
    {
        return imageRef.ToString("N", CultureInfo.InvariantCulture);
    }
}
