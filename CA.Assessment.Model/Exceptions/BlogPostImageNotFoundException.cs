using System.Runtime.Serialization;

namespace CA.Assessment.Model.Exceptions;

[Serializable]
public sealed class BlogPostImageNotFoundException : AssessmentDomainException
{
    public Guid NotFoundBlogPostImageIdentity { get; }

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

    public BlogPostImageNotFoundException(string message)
        : base(message)
    {
    }

    public BlogPostImageNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
