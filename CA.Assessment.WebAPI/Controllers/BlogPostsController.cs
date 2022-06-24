using System.Net.Mime;
using CA.Assessment.Application.Requests;
using CA.Assessment.WebAPI.Dtos;
using CA.Assessment.WebAPI.Filters;
using CA.Assessment.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, Type = typeof(IEnumerable<AssessmentValidationProblem>))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> NewBlogPostAsync(
        [FromBody] [SwaggerRequestBody("New Blog Post Data", Required = true)]
        NewBlogPostDto? newBlogPostDto)
    {
        if (newBlogPostDto is null)
        {
            return BadRequest();
        }

        var newBlogPostId = await _txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        return Ok(newBlogPostId);
    }

    [HttpGet("{blogPostId}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BlogPostDetailsDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetBlogPostAsync(
        [FromRoute] [SwaggerParameter("Blog Post UUID", Required = true)]
        Guid blogPostId)
    {
        var maybeBlogPost = await _txScriptsFacade.GetBlogPostAsync(blogPostId);

        if (maybeBlogPost is null)
        {
            return NotFound();
        }

        return Ok(maybeBlogPost);
    }

    [HttpDelete("{blogPostId}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteBlogPostAsync(
        [FromRoute] [SwaggerParameter("Blog Post UUID", Required = true)]
        Guid blogPostId)
    {
        await _txScriptsFacade.DeleteBlogPostAsync(blogPostId);

        return NoContent();
    }

    [HttpPatch("{blogPostId}")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, Type = typeof(IEnumerable<AssessmentValidationProblem>))]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateBlogPostAsync(
        [FromRoute] [SwaggerParameter("Blog Post UUID", Required = true)]
        Guid blogPostId,
        [FromBody] [SwaggerRequestBody("New Blog Post Data to update existing Blog Post", Required = true)]
        UpdateBlogPostDto? updateBlogPostDto)
    {
        if (updateBlogPostDto is null)
        {
            return BadRequest();
        }

        await _txScriptsFacade.UpdateBlogPostAsync(blogPostId, updateBlogPostDto);

        return NoContent();
    }

    [HttpPost("search")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<BlogPostSummaryDto>))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> SearchBlogPostsAsync(
        [FromBody] [SwaggerRequestBody("Blog Post search filters", Required = true)]
        SearchBlogPostFiltersDto? searchBlogPostsFilters)
    {
        if (searchBlogPostsFilters is null)
        {
            return BadRequest();
        }

        var searchResults = await _txScriptsFacade.SearchBlogPostsAsync(searchBlogPostsFilters);

        return Ok(searchResults);
    }

    [HttpDelete("{blogPostId}/tags")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RemoveTagsAsync(
        [FromRoute] [SwaggerParameter("Blog Post UUID", Required = true)]
        Guid blogPostId,
        [FromBody] [SwaggerRequestBody("Tags to remove from the Blog Post", Required = true)]
        IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return BadRequest();
        }

        await _txScriptsFacade.UntagBlogPostAsync(blogPostId, tags);

        return NoContent();
    }

    [HttpPut("{blogPostId}/tags")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> AddTagsAsync(
        [FromRoute] [SwaggerParameter("Blog Post UUID", Required = true)]
        Guid blogPostId,
        [FromBody] [SwaggerRequestBody("Tags to add to the Blog Post", Required = true)]
        IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return BadRequest();
        }

        await _txScriptsFacade.TagBlogPostAsync(blogPostId, tags);

        return NoContent();
    }

    [HttpPost("{blogPostId}/image")]
    [RequestFormLimits(MultipartBodyLengthLimit = 2 * 1024 * 1024)]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [Consumes("multipart/form-data")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UploadImageAsync(
        [FromRoute] [SwaggerParameter("Blog Post UUID", Required = true)]
        Guid blogPostId,
        [SwaggerRequestBody("Image File", Required = true)]
        IFormFile? image)
    {
        if (image is null)
        {
            return BadRequest();
        }

        var newImageId = await _txScriptsFacade.AttachImageToBlogPostAsync(blogPostId, image);

        return Ok(newImageId);
    }

    [HttpGet("{blogPostId}/image")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(FileResult))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Image.Jpeg, Type = typeof(FileResult))]
    public async Task<IActionResult> GetImageAsync(
        [FromRoute] [SwaggerParameter("Blog Post UUID", Required = true)]
        Guid blogPostId)
    {
        var image = await _txScriptsFacade.GetBlogPostImageDataAsync(blogPostId);

        if (image is null)
        {
            return NotFound();
        }

        return File(image.ImageStream, image.Mime);
    }
}
