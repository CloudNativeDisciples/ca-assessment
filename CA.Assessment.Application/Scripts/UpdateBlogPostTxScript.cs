using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Requests;
using CA.Assessment.Application.Services;
using CA.Assessment.Model;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Store;
using FluentValidation;

namespace CA.Assessment.Application.Scripts;

public sealed class UpdateBlogPostTxScript
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly CategoriesMaker _categoriesMaker;
    private readonly IDatabaseSessionManager _databaseSessionManager;
    private readonly TagsMaker _tagsMaker;
    private readonly IValidator<UpdateBlogPost> _updateBlogPostValidator;

    public UpdateBlogPostTxScript(IDatabaseSessionManager databaseSessionManager,
        TagsMaker tagsMaker,
        CategoriesMaker categoriesMaker,
        IValidator<UpdateBlogPost> updateBlogPostValidator,
        IBlogPostRepository blogPostRepository)
    {
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _tagsMaker = tagsMaker ?? throw new ArgumentNullException(nameof(tagsMaker));
        _categoriesMaker = categoriesMaker ?? throw new ArgumentNullException(nameof(categoriesMaker));
        _updateBlogPostValidator = updateBlogPostValidator ?? throw new ArgumentNullException(nameof(updateBlogPostValidator));
        _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
    }

    public async Task ExecuteAsync(UpdateBlogPost updateBlogPostData)
    {
        if (updateBlogPostData is null)
        {
            throw new ArgumentNullException(nameof(updateBlogPostData));
        }

        await _updateBlogPostValidator.ValidateAndThrowAsync(updateBlogPostData);

        await _databaseSessionManager.BeginTransactionAsync();

        var maybeOriginalBlogPost = await _blogPostRepository.GetAsync(updateBlogPostData.Id);

        if (maybeOriginalBlogPost is null)
        {
            throw new BlogPostNotFoundException(updateBlogPostData.Id);
        }

        try
        {
            var tags = maybeOriginalBlogPost.Tags;

            if (updateBlogPostData.Tags is not null)
            {
                var newTags = await _tagsMaker.GetOrCreateTagsByNameAsync(updateBlogPostData.Tags);

                tags = newTags.Select(t => t.Identity).ToList();
            }

            var category = maybeOriginalBlogPost.Category;

            if (updateBlogPostData.Category is not null)
            {
                var newCategory = await _categoriesMaker.GetOrCreateCategoryByNameAsync(updateBlogPostData.Category);

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

            await _blogPostRepository.UpdateAsync(updatedBlogPost);

            await _databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await _databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }
}
