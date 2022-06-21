using System.Runtime.Serialization;

namespace CA.Assessment.Model.Exceptions;

[Serializable]
public sealed class MissingCategoryException : AssessmentDomainException
{
    public Guid MissingCategoryIdentity { get; }

    private MissingCategoryException()
    {
    }

    public MissingCategoryException(Guid missingCategoryIdentity)
    {
        MissingCategoryIdentity = missingCategoryIdentity;
    }

    private MissingCategoryException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    public MissingCategoryException(string message)
        : base(message)
    {
    }

    public MissingCategoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
