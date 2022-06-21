using CA.Assessment.Application.Services;

namespace CA.Assessment.Images.Stores.FileSystem;

internal sealed class ImageFromFileSystemStore : IImageContent
{
    private readonly string _imageFullPath;

    public ImageFromFileSystemStore(Guid imageRef, string imageFullPath)
    {
        _imageFullPath = imageFullPath ?? throw new ArgumentNullException(nameof(imageFullPath));

        Ref = imageRef;
    }

    public Guid Ref { get; }

    public Stream OpenReadStream()
    {
        return new FileStream(_imageFullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
    }
}
