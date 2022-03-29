using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Repositories;

public interface IBlogPostRepository
{
    Task SaveAsync(BlogPost blogPost);

    Task<BlogPost?> GetAsync(Guid blogPostIdentity);

    Task DeleteAsync(Guid blogPostIdentity);

    Task UpdateAsync(BlogPost blogPost);
}
