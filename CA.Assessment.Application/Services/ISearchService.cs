using CA.Assessment.Application.Dtos;

namespace CA.Assessment.Application.Services;

public interface ISearchService
{
    public Task<IEnumerable<BlogPostSummary>> SearchBlogPostsAsync(SearchBlogPostsFilters filters);
}
