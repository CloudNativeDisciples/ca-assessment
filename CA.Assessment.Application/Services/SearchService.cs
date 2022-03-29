using CA.Assessment.Application.Dtos;
using CA.Assessment.Application.Mappers;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Services;

internal sealed class SearchService : ISearchService
{
    private readonly IBlogPostRepository blogPostRepository;
    private readonly IDatabaseSessionManager databaseSessionManager;

    public SearchService(
        IBlogPostRepository blogPostRepository,
        IDatabaseSessionManager databaseSessionManager)
    {
        this.blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
        this.databaseSessionManager =
            databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
    }

    public async Task<IEnumerable<BlogPostSummary>> SearchBlogPostsAsync(SearchBlogPostsFilters filters)
    {
        if (filters is null) throw new ArgumentNullException(nameof(filters));

        if (filters.None)
        {
            return Enumerable.Empty<BlogPostSummary>();
        }

        await databaseSessionManager.OpenConnectionAsync();

        try
        {
            var blogPostsFound = await blogPostRepository.SearchAsync(filters.Title, filters.Category, filters.Tags);

            return blogPostsFound;
        }
        finally
        {
            await databaseSessionManager.CloseConnectionAsync();
        }
    }
}
