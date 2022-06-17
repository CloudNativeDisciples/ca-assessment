using System.Globalization;
using Microsoft.Extensions.Options;

namespace CA.Assessment.Images.Stores.FileSystem;

internal class FileSystemImageStore : IImageStore
{
    private readonly IOptions<FileSystemImageStoreOptions> _config;

    public FileSystemImageStore(IOptions<FileSystemImageStoreOptions> config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public Task<IImageFromStore?> GetImageAsync(Guid imageRef)
    {
        EnsureImagesFolder();

        var imageFileInfo = GetImageFileInfo(imageRef);

        IImageFromStore? maybeImageFile = null;

        if (imageFileInfo.Exists)
        {
            maybeImageFile = new ImageFromFileSystemStore(imageRef, imageFileInfo.FullName);
        }

        return Task.FromResult(maybeImageFile);
    }

    public async Task SaveImageAsync(Guid imageRef, Stream imageDataStream)
    {
        if (imageDataStream == null)
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
        var imageFileName = GenerateImageFileName(imageRef);

        var imageFullPath = Path.Combine(_config.Value.Folder, imageFileName);

        return new FileInfo(imageFullPath);
    }

    private void EnsureImagesFolder()
    {
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