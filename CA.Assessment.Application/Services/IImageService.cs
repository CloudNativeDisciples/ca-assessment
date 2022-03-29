using CA.Assessment.Application.Dtos;

namespace CA.Assessment.Application.Services;

public interface IImageService
{
    Task AttachImageToBlogPostAsync(Guid newImageId, Guid blogPostId, NewBlogPostImage newBlogPostImage);

    Task<BlogPostImageData> GetBlogPostImageAsync(Guid blogPostId, Guid imageId);
}
