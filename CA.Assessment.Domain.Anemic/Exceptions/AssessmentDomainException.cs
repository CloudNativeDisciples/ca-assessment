using System.Runtime.Serialization;

namespace CA.Assessment.Domain.Anemic.Exceptions;

[Serializable]
public abstract class AssessmentDomainException : Exception
{
    protected AssessmentDomainException()
    {
    }

    protected AssessmentDomainException(string message) : base(message)
    {
    }

    protected AssessmentDomainException(string message, Exception inner) : base(message, inner)
    {
    }

    protected AssessmentDomainException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
