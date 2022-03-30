using System.Runtime.Serialization;

namespace CA.Assessment.Domain.Anemic.Exceptions;

[Serializable]
public sealed class UnauthorizedBlogPostDeletionException : AssessmentDomainException
{
    public UnauthorizedBlogPostDeletionException()
    {
    }

    public UnauthorizedBlogPostDeletionException(string message) : base(message)
    {
    }

    public UnauthorizedBlogPostDeletionException(string message, Exception inner) : base(message, inner)
    {
    }

    private UnauthorizedBlogPostDeletionException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
