using CA.Assessment.Application.Requests;
using CA.Assessment.WebAPI.Filters;
using CA.Assessment.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CA.Assessment.WebAPI.Controllers;

[ApiController]
[Route("api/v1/blog-posts")]
[TypeFilter(typeof(DomainExceptionFilters))]
public class BlogPostsController : ControllerBase
{
    private readonly TxScriptsFacade _txScriptsFacade;

    public BlogPostsController(TxScriptsFacade txScriptsFacade)
    {
        _txScriptsFacade = txScriptsFacade ?? throw new ArgumentNullException(nameof(txScriptsFacade));
    }

    [HttpPost]
    public async Task<IActionResult> NewBlogPostAsync([FromBody] NewBlogPost? newBlogPost)
    {
        if (newBlogPost is null)
        {
            return BadRequest();
        }

        var newBlogPostId = Guid.NewGuid();

        await _txScriptsFacade.NewBlogPostAsync(newBlogPostId, newBlogPost);

        return Ok(newBlogPostId);
    }

    [HttpGet("{blogPostId}")]
    public async Task<IActionResult> GetBlogPostAsync([FromRoute] Guid blogPostId)
    {
        var maybeBlogPost = await _txScriptsFacade.GetBlogPostAsync(blogPostId);

        if (maybeBlogPost is null)
        {
            return NotFound();
        }

        return Ok(maybeBlogPost);
    }

    [HttpDelete("{blogPostId}")]
    public async Task<IActionResult> DeleteBlogPostAsync([FromRoute] Guid blogPostId)
    {
        await _txScriptsFacade.DeleteBlogPostAsync(blogPostId);

        return NoContent();
    }

    [HttpPatch("{blogPostId}")]
    public async Task<IActionResult> UpdateBlogPostAsync([FromRoute] Guid blogPostId, [FromBody] UpdateBlogPost? updateBlogPost)
    {
        if (updateBlogPost is null)
        {
            return BadRequest();
        }

        await _txScriptsFacade.UpdateBlogPostAsync(blogPostId, updateBlogPost);

        return NoContent();
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchBlogPostsAsync([FromBody] SearchBlogPostsFilters? searchBlogPostsFilters)
    {
        if (searchBlogPostsFilters is null)
        {
            return BadRequest();
        }

        var searchResults = await _txScriptsFacade.SearchBlogPostsAsync(searchBlogPostsFilters);

        return Ok(searchResults);
    }

    [HttpDelete("{blogPostId}/tags")]
    public async Task<IActionResult> RemoveTagsAsync([FromRoute] Guid blogPostId, [FromBody] IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return BadRequest();
        }

        await _txScriptsFacade.UntagBlogPostAsync(blogPostId, tags);

        return NoContent();
    }

    [HttpPut("{blogPostId}/tags")]
    public async Task<IActionResult> AddTagsAsync([FromRoute] Guid blogPostId, [FromBody] IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return BadRequest();
        }

        await _txScriptsFacade.TagBlogPostAsync(blogPostId, tags);

        return NoContent();
    }

    [HttpPost("{blogPostId}/image")]
    public async Task<IActionResult> UploadImageAsync([FromRoute] Guid blogPostId, [FromForm] IFormFile? image)
    {
        if (image is null)
        {
            return BadRequest();
        }

        var newImageId = Guid.NewGuid();

        await using var imageStream = image.OpenReadStream();

        var newBlogPostImage = new BlogPostImageToAttach(image.Name, image.ContentType, imageStream);

        await _txScriptsFacade.AttachImageToBlogPostAsync(newImageId, blogPostId, newBlogPostImage);

        return Ok(newImageId);
    }

    [HttpGet("{blogPostId}/image")]
    public async Task<IActionResult> GetImageAsync([FromRoute] Guid blogPostId)
    {
        var image = await _txScriptsFacade.GetBlogPostImageDataAsync(blogPostId);

        if (image is null)
        {
            return NotFound();
        }

        return File(image.ImageStream, image.Mime);
    }
}
