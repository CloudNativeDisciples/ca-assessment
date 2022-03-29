using System.Runtime.Serialization;

namespace CA.Assessment.Domain.Anemic.Exceptions;

[Serializable]
public sealed class BlogPostImageNotFoundException : AssessmentDomainException
{
    private BlogPostImageNotFoundException()
    {
    }

    public BlogPostImageNotFoundException(Guid notFoundBlogPostImageIdentity)
    {
        NotFoundBlogPostImageIdentity = notFoundBlogPostImageIdentity;
    }

    private BlogPostImageNotFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    public Guid NotFoundBlogPostImageIdentity { get; }
}
