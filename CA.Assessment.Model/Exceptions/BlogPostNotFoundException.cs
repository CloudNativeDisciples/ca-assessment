using System.Runtime.Serialization;

namespace CA.Assessment.Model.Exceptions;

[Serializable]
public sealed class BlogPostNotFoundException : AssessmentDomainException
{
    public Guid NotFoundBlogPostIdentity { get; }

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

    public BlogPostNotFoundException(string message)
        : base(message)
    {
    }

    public BlogPostNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
