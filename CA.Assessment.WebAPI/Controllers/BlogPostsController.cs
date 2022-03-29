using CA.Assessment.Application.Dtos;
using CA.Assessment.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CA.Assessment.WebAPI.Controllers;

[ApiController]
[Route("api/v1/blog-posts")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostsService blogPostsService;
    private readonly ISearchService searchService;
    private readonly ITagsService tagsService;

    public BlogPostsController(
        IBlogPostsService blogPostsService,
        ISearchService searchService,
        ITagsService tagsService)
    {
        this.blogPostsService = blogPostsService ?? throw new ArgumentNullException(nameof(blogPostsService));
        this.searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        this.tagsService = tagsService ?? throw new ArgumentNullException(nameof(tagsService));
    }

    [HttpPost]
    public async Task<IActionResult> NewBlogPostAsync([FromBody] NewBlogPost? newBlogPost)
    {
        if (newBlogPost is null) return BadRequest();

        var newBlogPostId = Guid.NewGuid();

        await blogPostsService.NewAsync(newBlogPostId, newBlogPost);

        return Ok(newBlogPostId);
    }

    [HttpGet("{blogPostId}")]
    public async Task<IActionResult> GetBlogPostAsync([FromRoute] Guid blogPostId)
    {
        var maybeBlogPost = await blogPostsService.GetAsync(blogPostId);

        if (maybeBlogPost is null) return NotFound();

        return Ok(maybeBlogPost);
    }

    [HttpDelete("{blogPostId}")]
    public async Task<IActionResult> DeleteBlogPostAsync([FromRoute] Guid blogPostId)
    {
        await blogPostsService.DeleteAsync(blogPostId);

        return Ok();
    }

    [HttpPatch("{blogPostId}")]
    public async Task<IActionResult> UpdateBlogPostAsync(
        [FromRoute] Guid blogPostId,
        [FromBody] UpdateBlogPost? updateBlogPost)
    {
        if (updateBlogPost is null) return BadRequest();

        await blogPostsService.UpdateAsync(blogPostId, updateBlogPost);

        return Ok();
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchBlogPostsAsync([FromBody] SearchBlogPostsFilters? searchBlogPostsFilters)
    {
        if (searchBlogPostsFilters is null) return BadRequest();

        var searchResults = await searchService.SearchBlogPostsAsync(searchBlogPostsFilters);

        return Ok(searchResults);
    }

    [HttpDelete("{blogPostId}/tags")]
    public async Task<IActionResult> RemoveTagsAsync([FromRoute] Guid blogPostId, [FromBody] IEnumerable<string>? tags)
    {
        return Ok();
    }

    [HttpPut("{blogPostId}/tags")]
    public async Task<IActionResult> AddTagsAsync([FromRoute] Guid blogPostId, [FromBody] IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return BadRequest();
        }

        await tagsService.TagAsync(blogPostId, tags);

        return Ok();
    }

    [HttpPost("{blogPostId}/images")]
    public async Task<IActionResult> UploadImageAsync()
    {
        return Ok();
    }
}
