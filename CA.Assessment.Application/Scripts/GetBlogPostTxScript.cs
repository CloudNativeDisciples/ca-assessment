using CA.Assessment.Application.Mappers;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Responses;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Scripts;

public sealed class GetBlogPostTxScript
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDatabaseSessionManager _databaseSessionManager;
    private readonly ITagsRepository _tagsRepository;

    public GetBlogPostTxScript(IDatabaseSessionManager databaseSessionManager,
        IBlogPostRepository blogPostRepository,
        ITagsRepository tagsRepository,
        ICategoryRepository categoryRepository)
    {
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
        _tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<BlogPostDetails?> ExecuteAsync(Guid blogPostId)
    {
        await _databaseSessionManager.OpenConnectionAsync();

        try
        {
            var maybeBlogPost = await _blogPostRepository.GetAsync(blogPostId);

            if (maybeBlogPost is null)
            {
                return null;
            }

            var maybeCategory = await _categoryRepository.GetAsync(maybeBlogPost.Category);

            if (maybeCategory is null)
            {
                throw new MissingCategoryException(maybeBlogPost.Category);
            }

            var maybeTags = await _tagsRepository.GetManyAsync(maybeBlogPost.Tags);

            return BlogPostMapper.MapOneToBlogPostDetails(maybeBlogPost, maybeCategory, maybeTags);
        }
        finally
        {
            await _databaseSessionManager.CloseConnectionAsync();
        }
    }
}
