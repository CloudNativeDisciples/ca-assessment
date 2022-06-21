using CA.Assessment.Application.Responses;
using CA.Assessment.Model;

namespace CA.Assessment.Application.Mappers;

internal static class BlogPostMapper
{
    internal static BlogPostDetails MapOneToBlogPostDetails(BlogPost blogPost, Category category, IEnumerable<Tag> tags)
    {
        if (blogPost is null)
        {
            throw new ArgumentNullException(nameof(blogPost));
        }

        if (category is null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        var tagDetails = tags.Select(t => new TagDetails(t.Identity, t.Name))
            .ToList();

        var categoryDetails = new CategoryDetails(category.Identity, category.Name);

        return new BlogPostDetails(blogPost.Identity, blogPost.Author, blogPost.Content, blogPost.Title,
            blogPost.Image, categoryDetails, tagDetails);
    }
}
