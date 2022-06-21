using CA.Assessment.Application.Repositories;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Scripts;

public sealed class UntagBlogPostTxScript
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IDatabaseSessionManager _databaseSessionManager;
    private readonly ITagsRepository _tagsRepository;

    public UntagBlogPostTxScript(IDatabaseSessionManager databaseSessionManager,
        IBlogPostRepository blogPostRepository,
        ITagsRepository tagsRepository)
    {
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
        _tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
    }

    public async Task ExecuteAsync(Guid blogPostId, IEnumerable<string> tags)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        await _databaseSessionManager.BeginTransactionAsync();

        try
        {
            var maybeBlogPost = await _blogPostRepository.GetAsync(blogPostId);

            if (maybeBlogPost is null)
            {
                throw new BlogPostNotFoundException(blogPostId);
            }

            var existingTags = await _tagsRepository.GetManyAsync(maybeBlogPost.Tags);

            var tagsToRemove = existingTags.Where(t => tags.Contains(t.Name));

            await _tagsRepository.RemoveTagsFromBlogPostAsync(blogPostId, tagsToRemove);

            await _databaseSessionManager.CommitTransactionAsync();
        }
        finally
        {
            await _databaseSessionManager.RollbackTransactionAsync();
        }
    }
}
