using System.Runtime.Serialization;

namespace CA.Assessment.Domain.Anemic.Exceptions;

[Serializable]
public sealed class BlogPostNotFoundException : AssessmentDomainException
{
    private BlogPostNotFoundException()
    {
    }

    public BlogPostNotFoundException(Guid notFoundBlogPostIdentity)
    {
        NotFoundBlogPostIdentity = notFoundBlogPostIdentity;
    }

    private BlogPostNotFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    public Guid NotFoundBlogPostIdentity { get; }
}
