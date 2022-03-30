using System.Runtime.Serialization;

namespace CA.Assessment.Domain.Anemic.Exceptions;

[Serializable]
public sealed class ForbiddenBlogPostDeletionException : AssessmentDomainException
{
    public ForbiddenBlogPostDeletionException()
    {
    }

    public ForbiddenBlogPostDeletionException(string message) : base(message)
    {
    }

    public ForbiddenBlogPostDeletionException(string message, Exception inner) : base(message, inner)
    {
    }

    private ForbiddenBlogPostDeletionException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
