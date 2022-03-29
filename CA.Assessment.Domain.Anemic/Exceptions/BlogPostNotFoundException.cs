using System.Runtime.Serialization;

namespace CA.Assessment.Domain.Anemic.Exceptions;

[Serializable]
public class BlogPostNotFoundException : AssessmentDomainException
{
    private BlogPostNotFoundException()
    {
    }

    public BlogPostNotFoundException(Guid notFoundBlogPostIdentity)
    {
        NotFoundBlogPostIdentity = notFoundBlogPostIdentity;
    }

    protected BlogPostNotFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    public Guid NotFoundBlogPostIdentity { get; }
}
