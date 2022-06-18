using CA.Assessment.Application.Dtos;
using CA.Assessment.Database.Sqlite.Rows;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal sealed class BlogPostSummaryRowMapper
{
    private BlogPostSummary MapOne(BlogPostSummaryRow blogPostSummaryRow)
    {
        if (blogPostSummaryRow is null)
        {
            throw new ArgumentNullException(nameof(blogPostSummaryRow));
        }

        return new BlogPostSummary(Guid.Parse(blogPostSummaryRow.Id), blogPostSummaryRow.Title);
    }

    public IEnumerable<BlogPostSummary> MapMany(IEnumerable<BlogPostSummaryRow> blogPostSummaryRows)
    {
        if (blogPostSummaryRows is null)
        {
            throw new ArgumentNullException(nameof(blogPostSummaryRows));
        }

        return blogPostSummaryRows.Select(MapOne).ToList();
    }
}