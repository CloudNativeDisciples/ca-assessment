using CA.Assessment.Images.Stores.FileSystem;

namespace CA.Assessment.Images;

public interface IImageStore
{
    Task<IImageFromStore?> GetImageAsync(Guid imageRef);

    Task SaveImageAsync(Guid imageRef, Stream imageDataStream);
}