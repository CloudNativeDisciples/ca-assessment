using CA.Assessment.Application.Dtos;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Mappers;

internal sealed class ImageMapper
{
    internal BlogPostImageData MapOneToBlogPostImageData(Image blogPostImage)
    {
        if (blogPostImage is null) throw new ArgumentNullException(nameof(blogPostImage));

        return new BlogPostImageData(blogPostImage.Content, blogPostImage.Mime);
    }
}
