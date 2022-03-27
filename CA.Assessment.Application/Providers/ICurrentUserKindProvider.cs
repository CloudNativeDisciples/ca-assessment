using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Providers;

public interface ICurrentUserKindProvider
{
    Task<UserKind> GetUserKindAsync();
}
