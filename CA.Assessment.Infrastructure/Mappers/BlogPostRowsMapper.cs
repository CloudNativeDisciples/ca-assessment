using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Rows;

namespace CA.Assessment.Infrastructure.Mappers;

internal sealed class BlogPostRowsMapper
{
    public BlogPost MapOne(BlogPostRow blogPostRow, IEnumerable<BlogPostToTagRow> blogPostToTagRows)
    {
        if (blogPostRow is null) throw new ArgumentNullException(nameof(blogPostRow));
        if (blogPostToTagRows is null) throw new ArgumentNullException(nameof(blogPostToTagRows));

        var tags = blogPostToTagRows
            .Select(t => Guid.Parse(t.TagId))
            .ToList();

        return new BlogPost(Guid.Parse(blogPostRow.Id), blogPostRow.Title, blogPostRow.Author, blogPostRow.Content,
            Guid.Parse(blogPostRow.ImageId), tags, Guid.Parse(blogPostRow.CategoryId));
    }
}
