using CA.Assessment.Model;

namespace CA.Assessment.Application.Repositories;

public interface IImagesRepository
{
    Task SaveAsync(BlogPostImage image);

    Task<BlogPostImage?> GetAsync(Guid imageId);
}
