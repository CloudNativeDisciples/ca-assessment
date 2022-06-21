using CA.Assessment.Application.Responses;
using CA.Assessment.Application.Services;
using CA.Assessment.Model;

namespace CA.Assessment.Application.Mappers;

internal static class ImageMapper
{
    internal static BlogPostImageData MapOneToBlogPostImageData(BlogPostImage blogPostImage, IImageContent imageStream)
    {
        if (blogPostImage is null)
        {
            throw new ArgumentNullException(nameof(blogPostImage));
        }

        if (imageStream is null)
        {
            throw new ArgumentNullException(nameof(imageStream));
        }

        return new BlogPostImageData(blogPostImage.Mime, imageStream.OpenReadStream());
    }
}
