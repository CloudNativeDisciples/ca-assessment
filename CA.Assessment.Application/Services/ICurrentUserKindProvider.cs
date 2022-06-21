using CA.Assessment.Model;

namespace CA.Assessment.Application.Services;

public interface ICurrentUserKindProvider
{
    Task<UserKind> GetUserKindAsync();
}
