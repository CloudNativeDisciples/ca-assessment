using CA.Assessment.Application.Requests;
using CA.Assessment.Application.Responses;
using CA.Assessment.Application.Scripts;

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

    public async Task AttachImageToBlogPostAsync(Guid newImageId, Guid blogPostId, BlogPostImageToAttach newBlogPostImage)
    {
        await _attachBlogPostImageTxScript.ExecuteAsync(newImageId, blogPostId, newBlogPostImage);
    }

    public async Task TagBlogPostAsync(Guid blogPostId, IEnumerable<string> tags)
    {
        await _tagBlogPostTxScript.ExecuteAsync(blogPostId, tags);
    }

    public async Task UntagBlogPostAsync(Guid blogPostId, IEnumerable<string> tags)
    {
        await _tagBlogPostTxScript.ExecuteAsync(blogPostId, tags);
    }

    public async Task<IEnumerable<BlogPostSummary>?> SearchBlogPostsAsync(SearchBlogPostsFilters searchBlogPostsFilters)
    {
        return await _searchBlogPostsTxScript.ExecuteAsync(searchBlogPostsFilters);
    }

    public async Task UpdateBlogPostAsync(Guid blogPostId, UpdateBlogPost updateBlogPost)
    {
        await _updateBlogPostTxScript.ExecuteAsync(blogPostId, updateBlogPost);
    }

    public async Task DeleteBlogPostAsync(Guid blogPostId)
    {
        await _deleteBlogPostTxScript.ExecuteAsync(blogPostId);
    }

    public async Task NewBlogPostAsync(Guid newBlogPostId, NewBlogPost newBlogPost)
    {
        await _newBlogPostTxScript.ExecuteAsync(newBlogPostId, newBlogPost);
    }

    public async Task<BlogPostDetails?> GetBlogPostAsync(Guid blogPostId)
    {
        return await _getBlogPostTxScript.ExecuteAsync(blogPostId);
    }
}
