namespace CA.Assessment.Images.Stores.FileSystem;

public sealed class ImageFromFileSystemStore : IImageFromStore
{
    private readonly string _imageFullPath;

    public Guid Ref { get; }

    public ImageFromFileSystemStore(Guid imageRef, string imageFullPath)
    {
        _imageFullPath = imageFullPath ?? throw new ArgumentNullException(nameof(imageFullPath));

        Ref = imageRef;
    }

    public Stream OpenReadStream()
    {
        return new FileStream(_imageFullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
            FileOptions.Asynchronous);
    }
}