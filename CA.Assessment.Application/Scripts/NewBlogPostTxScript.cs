using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Requests;
using CA.Assessment.Application.Services;
using CA.Assessment.Model;
using CA.Assessment.Store;
using FluentValidation;

namespace CA.Assessment.Application.Scripts;

public sealed class NewBlogPostTxScript
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly CategoriesMaker _categoriesMaker;
    private readonly IDatabaseSessionManager _databaseSessionManager;
    private readonly IValidator<NewBlogPost> _newBlogPostValidator;
    private readonly TagsMaker _tagsMaker;

    public NewBlogPostTxScript(IDatabaseSessionManager databaseSessionManager,
        IBlogPostRepository blogPostRepository,
        TagsMaker tagsMaker,
        CategoriesMaker categoriesMaker,
        IValidator<NewBlogPost> newBlogPostValidator)
    {
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
        _tagsMaker = tagsMaker ?? throw new ArgumentNullException(nameof(tagsMaker));
        _categoriesMaker = categoriesMaker ?? throw new ArgumentNullException(nameof(categoriesMaker));
        _newBlogPostValidator = newBlogPostValidator ?? throw new ArgumentNullException(nameof(newBlogPostValidator));
    }

    public async Task ExecuteAsync(NewBlogPost newBlogPostData)
    {
        if (newBlogPostData is null)
        {
            throw new ArgumentNullException(nameof(newBlogPostData));
        }

        await _newBlogPostValidator.ValidateAndThrowAsync(newBlogPostData);

        await _databaseSessionManager.BeginTransactionAsync();

        try
        {
            var blogPostTags = await _tagsMaker.GetOrCreateTagsByNameAsync(newBlogPostData.Tags!);

            var blogPostTagIdentities = blogPostTags
                .Select(t => t.Identity)
                .ToList();

            var blogPostCategory = await _categoriesMaker.GetOrCreateCategoryByNameAsync(newBlogPostData.Category!);

            var newBlogPost = new BlogPost(newBlogPostData.Id, newBlogPostData.Title!, newBlogPostData.Author!,
                newBlogPostData.Content!, Guid.Empty, blogPostTagIdentities, blogPostCategory.Identity);

            await _blogPostRepository.SaveAsync(newBlogPost);

            await _databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await _databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }
}
