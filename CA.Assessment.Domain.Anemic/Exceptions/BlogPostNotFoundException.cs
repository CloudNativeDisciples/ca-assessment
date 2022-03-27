using System.Runtime.Serialization;

namespace CA.Assessment.Domain.Anemic.Exceptions;

[Serializable]
public class BlogPostNotFoundException : AssessmentDomainException
{
    public Guid NotFoundBlogPostIdentity { get; }

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
}
