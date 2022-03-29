using CA.Assessment.Application.Dtos;

namespace CA.Assessment.Application.Services;

public interface IBlogPostsService
{
    Task NewAsync(Guid newBlogPostId, NewBlogPost newBlogPostData);

    Task<BlogPostDetail?> GetAsync(Guid blogPostId);

    Task DeleteAsync(Guid blogPostIdentity);

    Task UpdateAsync(Guid blogPostId, UpdateBlogPost updateBlogPostData);
}
