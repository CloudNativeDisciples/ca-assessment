using CA.Assessment.Application.Providers;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic.Exceptions;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Services;

internal sealed class TagsService : ITagsService
{
    private readonly IBlogPostRepository blogPostRepository;
    private readonly IDatabaseSessionManager databaseSessionManager;
    private readonly ITagsMaker tagsMaker;
    private readonly ITagsRepository tagsRepository;

    public TagsService(
        IDatabaseSessionManager databaseSessionManager,
        ITagsRepository tagsRepository,
        IBlogPostRepository blogPostRepository,
        ITagsMaker tagsMaker)
    {
        this.databaseSessionManager =
            databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));

        this.tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));

        this.blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));

        this.tagsMaker = tagsMaker ?? throw new ArgumentNullException(nameof(tagsMaker));
    }

    public async Task TagAsync(Guid blogPostId, IEnumerable<string> tags)
    {
        if (tags is null) throw new ArgumentNullException(nameof(tags));

        await databaseSessionManager.BeginTransactionAsync();

        try
        {
            var maybeBlogPost = await blogPostRepository.GetAsync(blogPostId);

            if (maybeBlogPost is null) throw new BlogPostNotFoundException(blogPostId);

            var existingTags = await tagsRepository.GetManyAsync(maybeBlogPost.Tags);

            var existingTagIdentities = existingTags.Select(t => t.Identity).ToList();

            var newTags = await tagsMaker.GetOrCreateTagsByNameAsync(tags);

            var tagsToAdd = newTags.ExceptBy(existingTagIdentities, t => t.Identity);

            await tagsRepository.AddTagsToBlogPostAsync(blogPostId, tagsToAdd);

            await databaseSessionManager.CommitTransactionAsync();
        }
        finally
        {
            await databaseSessionManager.RollbackTransactionAsync();
        }
    }

    public async Task UntagAsync(Guid blogPostId, IEnumerable<string> tags)
    {
        if (tags is null) throw new ArgumentNullException(nameof(tags));

        await databaseSessionManager.BeginTransactionAsync();

        try
        {
            var maybeBlogPost = await blogPostRepository.GetAsync(blogPostId);

            if (maybeBlogPost is null) throw new BlogPostNotFoundException(blogPostId);

            var existingTags = await tagsRepository.GetManyAsync(maybeBlogPost.Tags);

            var tagsToRemove = existingTags.Where(t => tags.Contains(t.Name));

            await tagsRepository.RemoveTagsToBlogPostAsync(blogPostId, tagsToRemove);

            await databaseSessionManager.CommitTransactionAsync();
        }
        finally
        {
            await databaseSessionManager.RollbackTransactionAsync();
        }
    }
}
