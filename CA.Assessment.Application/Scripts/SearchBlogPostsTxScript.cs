using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Requests;
using CA.Assessment.Application.Responses;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Scripts;

public sealed class SearchBlogPostsTxScript
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IDatabaseSessionManager _databaseSessionManager;

    public SearchBlogPostsTxScript(IDatabaseSessionManager databaseSessionManager,
        IBlogPostRepository blogPostRepository)
    {
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
    }

    public async Task<IEnumerable<BlogPostSummary>> ExecuteAsync(SearchBlogPost filters)
    {
        if (filters is null)
        {
            throw new ArgumentNullException(nameof(filters));
        }

        if (filters.None)
        {
            return Enumerable.Empty<BlogPostSummary>();
        }

        await _databaseSessionManager.OpenConnectionAsync();

        try
        {
            return await _blogPostRepository.SearchAsync(filters.Title, filters.Category, filters.Tags);
        }
        finally
        {
            await _databaseSessionManager.CloseConnectionAsync();
        }
    }
}
