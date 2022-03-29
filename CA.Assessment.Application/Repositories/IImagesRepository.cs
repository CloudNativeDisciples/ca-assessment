using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Repositories;

public interface IImagesRepository
{
    Task SaveAsync(Image image);

    Task<Image?> GetAsync(Guid imageId);
}
