using CA.Assessment.Application.Dtos;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Images;

namespace CA.Assessment.Application.Mappers;

internal sealed class ImageMapper
{
    internal BlogPostImageData MapOneToBlogPostImageData(Image blogPostImage, IImageFromStore imageStream)
    {
        if (blogPostImage is null)
        {
            throw new ArgumentNullException(nameof(blogPostImage));
        }

        if (imageStream == null)
        {
            throw new ArgumentNullException(nameof(imageStream));
        }

        return new BlogPostImageData(blogPostImage.Mime, imageStream.OpenReadStream());
    }
}