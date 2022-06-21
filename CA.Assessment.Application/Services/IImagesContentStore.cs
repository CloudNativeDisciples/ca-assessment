namespace CA.Assessment.Application.Services;

public interface IImagesContentStore
{
    Task<IImageContent?> GetContentAsync(Guid imageRef);

    Task SaveContentAsync(Guid imageRef, Stream imageDataStream);
}
