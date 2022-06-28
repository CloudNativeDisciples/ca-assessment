using CA.Assessment.Application.Requests;
using CA.Assessment.Application.Responses;
using CA.Assessment.Application.Scripts;
using CA.Assessment.WebAPI.Dtos;

namespace CA.Assessment.WebAPI.Services;

public sealed class TxScriptsFacade
{
    private readonly AttachBlogPostImageTxScript _attachBlogPostImageTxScript;
    private readonly DeleteBlogPostTxScript _deleteBlogPostTxScript;
    private readonly GetBlogPostImageDataTxScript _getBlogPostImageDataTxScript;
    private readonly GetBlogPostTxScript _getBlogPostTxScript;
    private readonly NewBlogPostTxScript _newBlogPostTxScript;
    private readonly SearchBlogPostsTxScript _searchBlogPostsTxScript;
    private readonly TagBlogPostTxScript _tagBlogPostTxScript;
    private readonly UntagBlogPostTxScript _untagBlogPostTxScript;
    private readonly UpdateBlogPostTxScript _updateBlogPostTxScript;

    public TxScriptsFacade(
        NewBlogPostTxScript newBlogPostTxScript,
        GetBlogPostTxScript getBlogPostTxScript,
        DeleteBlogPostTxScript deleteBlogPostTxScript,
        UpdateBlogPostTxScript updateBlogPostTxScript,
        SearchBlogPostsTxScript searchBlogPostsTxScript,
        UntagBlogPostTxScript untagBlogPostTxScript,
        TagBlogPostTxScript tagBlogPostTxScript,
        AttachBlogPostImageTxScript attachBlogPostImageTxScript,
        GetBlogPostImageDataTxScript getBlogPostImageDataTxScript
    )
    {
        _newBlogPostTxScript = newBlogPostTxScript ?? throw new ArgumentNullException(nameof(newBlogPostTxScript));
        _getBlogPostTxScript = getBlogPostTxScript ?? throw new ArgumentNullException(nameof(getBlogPostTxScript));
        _deleteBlogPostTxScript = deleteBlogPostTxScript ?? throw new ArgumentNullException(nameof(deleteBlogPostTxScript));
        _updateBlogPostTxScript = updateBlogPostTxScript ?? throw new ArgumentNullException(nameof(updateBlogPostTxScript));
        _searchBlogPostsTxScript = searchBlogPostsTxScript ?? throw new ArgumentNullException(nameof(searchBlogPostsTxScript));
        _untagBlogPostTxScript = untagBlogPostTxScript ?? throw new ArgumentNullException(nameof(untagBlogPostTxScript));
        _tagBlogPostTxScript = tagBlogPostTxScript ?? throw new ArgumentNullException(nameof(tagBlogPostTxScript));
        _attachBlogPostImageTxScript = attachBlogPostImageTxScript ?? throw new ArgumentNullException(nameof(attachBlogPostImageTxScript));
        _getBlogPostImageDataTxScript = getBlogPostImageDataTxScript ?? throw new ArgumentNullException(nameof(getBlogPostImageDataTxScript));
    }


    public async Task<BlogPostImageData?> GetBlogPostImageDataAsync(Guid blogPostId)
    {
        return await _getBlogPostImageDataTxScript.ExecuteAsync(blogPostId);
    }

    public async Task<Guid> AttachImageToBlogPostAsync(Guid blogPostId, IFormFile image)
    {
        if (image is null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        var newImageId = Guid.NewGuid();

        await using var imageStream = image.OpenReadStream();

        var newBlogPostImage = new AttachImageToBlogPost(blogPostId, newImageId, image.Name, image.ContentType, imageStream);
        
        await _attachBlogPostImageTxScript.ExecuteAsync(newBlogPostImage);

        return newImageId;
    }

    public async Task TagBlogPostAsync(Guid blogPostId, IEnumerable<string> tags)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        await _tagBlogPostTxScript.ExecuteAsync(blogPostId, tags);
    }

    public async Task UntagBlogPostAsync(Guid blogPostId, IEnumerable<string> tags)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        await _untagBlogPostTxScript.ExecuteAsync(blogPostId, tags);
    }

    public async Task<IEnumerable<BlogPostSummaryDto>> SearchBlogPostsAsync(SearchBlogPostFiltersDto filters)
    {
        if (filters is null)
        {
            throw new ArgumentNullException(nameof(filters));
        }

        var searchBlogPostData = new SearchBlogPost(filters.Category, filters.Tags, filters.Title);

        var searchResults = await _searchBlogPostsTxScript.ExecuteAsync(searchBlogPostData);

        return searchResults
            .Select(r => new BlogPostSummaryDto(r.Identity, r.Title))
            .ToList();
    }

    public async Task UpdateBlogPostAsync(Guid blogPostId, UpdateBlogPostDto updateBlogPostDto)
    {
        if (updateBlogPostDto is null)
        {
            throw new ArgumentNullException(nameof(updateBlogPostDto));
        }

        var updateBlogPost = new UpdateBlogPost(blogPostId, updateBlogPostDto.Title, updateBlogPostDto.Content,
            updateBlogPostDto.Author, updateBlogPostDto.Category, updateBlogPostDto.Tags);

        await _updateBlogPostTxScript.ExecuteAsync(updateBlogPost);
    }

    public async Task DeleteBlogPostAsync(Guid blogPostId)
    {
        await _deleteBlogPostTxScript.ExecuteAsync(blogPostId);
    }

    public async Task<Guid> NewBlogPostAsync(NewBlogPostDto newBlogPostDto)
    {
        if (newBlogPostDto is null)
        {
            throw new ArgumentNullException(nameof(newBlogPostDto));
        }

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostData = new NewBlogPost(newBlogPostId, newBlogPostDto.Title, newBlogPostDto.Content,
            newBlogPostDto.Author, newBlogPostDto.Category, newBlogPostDto.Tags);

        await _newBlogPostTxScript.ExecuteAsync(newBlogPostData);

        return newBlogPostId;
    }

    public async Task<BlogPostDetailsDto?> GetBlogPostAsync(Guid blogPostId)
    {
        var blogPostDetails = await _getBlogPostTxScript.ExecuteAsync(blogPostId);

        if (blogPostDetails is null)
        {
            return null;
        }

        var categoryDetailsDto = new CategoryDetailsDto(blogPostDetails.Category.Identity, blogPostDetails.Category.Name);

        var tagDetailDtos = blogPostDetails.Tags
            .Select(t => new TagDetailsDto(t.Identity, t.Name));

        return new BlogPostDetailsDto(blogPostDetails.Identity, blogPostDetails.Author, blogPostDetails.Content, blogPostDetails.Title,
            blogPostDetails.Image, categoryDetailsDto, tagDetailDtos);
    }
}
