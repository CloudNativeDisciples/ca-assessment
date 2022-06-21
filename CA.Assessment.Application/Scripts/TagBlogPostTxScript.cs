using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Services;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Scripts;

public sealed class TagBlogPostTxScript
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IDatabaseSessionManager _databaseSessionManager;
    private readonly TagsMaker _tagsMaker;
    private readonly ITagsRepository _tagsRepository;

    public TagBlogPostTxScript(IDatabaseSessionManager databaseSessionManager,
        IBlogPostRepository blogPostRepository,
        ITagsRepository tagsRepository,
        TagsMaker tagsMaker)
    {
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
        _tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
        _tagsMaker = tagsMaker ?? throw new ArgumentNullException(nameof(tagsMaker));
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

            var existingTagIdentities = existingTags.Select(t => t.Identity).ToList();

            var newTags = await _tagsMaker.GetOrCreateTagsByNameAsync(tags);

            var tagsToAdd = newTags.ExceptBy(existingTagIdentities, t => t.Identity);

            await _tagsRepository.AddTagsToBlogPostAsync(blogPostId, tagsToAdd);

            await _databaseSessionManager.CommitTransactionAsync();
        }
        finally
        {
            await _databaseSessionManager.RollbackTransactionAsync();
        }
    }
}
