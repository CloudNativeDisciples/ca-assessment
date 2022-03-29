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

    public BlogPostsController(IBlogPostsService blogPostsService, ISearchService searchService)
    {
        this.blogPostsService = blogPostsService ?? throw new ArgumentNullException(nameof(blogPostsService));
        this.searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
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
        if (searchBlogPostsFilters is null)
        {
            return BadRequest();
        }

        var searchResults = await searchService.SearchBlogPostsAsync(searchBlogPostsFilters);

        return Ok(searchResults);
    }

    [HttpDelete("tags/{blogPostId}")]
    public async Task<IActionResult> RemoveTagsAsync()
    {
        return Ok();
    }

    [HttpPut("tags/{blogPostId}")]
    public async Task<IActionResult> AddTagsAsync()
    {
        return Ok();
    }

    [HttpPost("images")]
    public async Task<IActionResult> UploadImageAsync()
    {
        return Ok();
    }
}
