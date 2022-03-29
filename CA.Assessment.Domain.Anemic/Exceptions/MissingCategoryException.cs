using System.Runtime.Serialization;

namespace CA.Assessment.Domain.Anemic.Exceptions;

[Serializable]
public sealed class MissingCategoryException : AssessmentDomainException
{
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

    public Guid MissingCategoryIdentity { get; }
}
