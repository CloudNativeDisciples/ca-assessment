using CA.Assessment.Application.Dtos;
using CA.Assessment.Application.Mappers;
using CA.Assessment.Application.Providers;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Domain.Anemic.Exceptions;
using CA.Assessment.Store;
using FluentValidation;

namespace CA.Assessment.Application.Services;

internal sealed class BlogPostService : IBlogPostsService
{
    private readonly BlogPostMapper blogPostMapper;
    private readonly IBlogPostRepository blogPostRepository;
    private readonly ICategoriesMaker categoriesMaker;
    private readonly ICurrentUserKindProvider currentUserKindProvider;
    private readonly IDatabaseSessionManager databaseSessionManager;
    private readonly IValidator<NewBlogPost> newBlogPostValidator;
    private readonly ITagsMaker tagsMaker;
    private readonly IValidator<UpdateBlogPost> updateBlogPostValidator;

    public BlogPostService(
        IBlogPostRepository blogPostRepository,
        ITagsMaker tagsMaker,
        ICategoriesMaker categoriesMaker,
        IValidator<NewBlogPost> newBlogPostValidator,
        IValidator<UpdateBlogPost> updateBlogPostValidator,
        IDatabaseSessionManager databaseSessionManager,
        BlogPostMapper blogPostMapper,
        ICurrentUserKindProvider currentUserKindProvider)
    {
        this.blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));

        this.newBlogPostValidator =
            newBlogPostValidator ?? throw new ArgumentNullException(nameof(newBlogPostValidator));

        this.updateBlogPostValidator =
            updateBlogPostValidator ?? throw new ArgumentNullException(nameof(updateBlogPostValidator));

        this.databaseSessionManager = databaseSessionManager ??
                                      throw new ArgumentNullException(nameof(databaseSessionManager));

        this.blogPostMapper = blogPostMapper ?? throw new ArgumentNullException(nameof(blogPostMapper));

        this.currentUserKindProvider =
            currentUserKindProvider ?? throw new ArgumentNullException(nameof(currentUserKindProvider));

        this.tagsMaker = tagsMaker ?? throw new ArgumentNullException(nameof(tagsMaker));

        this.categoriesMaker = categoriesMaker ?? throw new ArgumentNullException(nameof(categoriesMaker));
    }

    public async Task NewAsync(Guid newBlogPostId, NewBlogPost newBlogPostData)
    {
        if (newBlogPostData is null) throw new ArgumentNullException(nameof(newBlogPostData));

        await newBlogPostValidator.ValidateAndThrowAsync(newBlogPostData);

        await databaseSessionManager.BeginTransactionAsync();

        var blogPostTags = await tagsMaker.GetOrCreateTagsByNameAsync(newBlogPostData.Tags);

        var blogPostTagIdentities = blogPostTags.Select(t => t.Identity);

        var blogPostCategory = await categoriesMaker.GetOrCreateCategoryByNameAsync(newBlogPostData.Category);

        var newBlogPost = new BlogPost(newBlogPostId, newBlogPostData.Title, newBlogPostData.Author,
            newBlogPostData.Content, Guid.Empty, blogPostTagIdentities, blogPostCategory.Identity);

        try
        {
            await blogPostRepository.SaveAsync(newBlogPost);

            await databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task UpdateAsync(Guid blogPostId, UpdateBlogPost updateBlogPostData)
    {
        if (updateBlogPostData is null) throw new ArgumentNullException(nameof(updateBlogPostData));

        await updateBlogPostValidator.ValidateAndThrowAsync(updateBlogPostData);

        await databaseSessionManager.BeginTransactionAsync();

        var maybeOriginalBlogPost = await blogPostRepository.GetAsync(blogPostId);

        if (maybeOriginalBlogPost is null) throw new BlogPostNotFoundException(blogPostId);

        //TODO: Maybe would look nicer encapsulating update logic somewhere more meaningful like IBlogPostUpdater

        var tags = maybeOriginalBlogPost.Tags;

        if (updateBlogPostData.Tags is not null)
        {
            var newTags = await tagsMaker.GetOrCreateTagsByNameAsync(updateBlogPostData.Tags);

            tags = newTags.Select(t => t.Identity).ToList();
        }

        var category = maybeOriginalBlogPost.Category;

        if (updateBlogPostData.Category is not null)
        {
            var newCategory = await categoriesMaker.GetOrCreateCategoryByNameAsync(updateBlogPostData.Category);

            category = newCategory.Identity;
        }

        var updatedBlogPost = new BlogPost(
            maybeOriginalBlogPost.Identity,
            updateBlogPostData.Title ?? maybeOriginalBlogPost.Title,
            updateBlogPostData.Author ?? maybeOriginalBlogPost.Author,
            updateBlogPostData.Content ?? maybeOriginalBlogPost.Content,
            maybeOriginalBlogPost.Image,
            tags,
            category
        );

        try
        {
            await blogPostRepository.UpdateAsync(updatedBlogPost);

            await databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<BlogPostDetail?> GetAsync(Guid blogPostIdentity)
    {
        await databaseSessionManager.OpenConnectionAsync();

        try
        {
            var maybeBlogPost = await blogPostRepository.GetAsync(blogPostIdentity);

            if (maybeBlogPost is null) return null;

            return await blogPostMapper.MapOneToBlogPostDetailsAsync(maybeBlogPost);
        }
        finally
        {
            await databaseSessionManager.CloseConnectionAsync();
        }
    }

    public async Task DeleteAsync(Guid blogPostIdentity)
    {
        var currentUserKind = await currentUserKindProvider.GetUserKindAsync();

        if (currentUserKind == UserKind.Unknown) throw new UnauthorizedBlogPostDeletionException();
        if (currentUserKind == UserKind.User) throw new ForbiddenBlogPostDeletionException();

        await databaseSessionManager.BeginTransactionAsync();

        try
        {
            await blogPostRepository.DeleteAsync(blogPostIdentity);

            await databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }
}
