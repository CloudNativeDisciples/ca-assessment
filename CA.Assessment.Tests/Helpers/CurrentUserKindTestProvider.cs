using System.Threading.Tasks;
using CA.Assessment.Application.Providers;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Tests.Helpers;

internal sealed class CurrentUserKindTestProvider : ICurrentUserKindProvider
{
    public UserKind CurrentUserKind { get; set; }

    public Task<UserKind> GetUserKindAsync()
    {
        return Task.FromResult(CurrentUserKind);
    }
}
