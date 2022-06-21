using CA.Assessment.Application.Services;
using CA.Assessment.Model;

namespace CA.Assessment.Tests.Helpers;

internal sealed class CurrentUserKindTestProvider : ICurrentUserKindProvider
{
    public UserKind CurrentUserKind { get; set; }

    public Task<UserKind> GetUserKindAsync()
    {
        return Task.FromResult(CurrentUserKind);
    }
}
