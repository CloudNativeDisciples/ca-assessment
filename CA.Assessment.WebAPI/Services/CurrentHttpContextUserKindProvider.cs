using CA.Assessment.Application.Providers;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.WebAPI.Services;

public sealed class CurrentHttpContextUserKindProvider : ICurrentUserKindProvider
{
    private const string X_USER_HEADER = "X-User";
    private const string X_USER_ADMIN_VALUE = "admin";
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentHttpContextUserKindProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Task<UserKind> GetUserKindAsync()
    {
        if (httpContextAccessor.HttpContext is null) return Task.FromResult(UserKind.Unknown);

        var xUserHeaderValue = httpContextAccessor.HttpContext.Request.Headers[X_USER_HEADER];
        var xUserRawValue = xUserHeaderValue.SingleOrDefault() ?? string.Empty;
        var hasXUserHeader = xUserHeaderValue.Count == 1 && string.IsNullOrWhiteSpace(xUserRawValue) is false;

        if (hasXUserHeader)
        {
            var isAdminUser = string.Equals(X_USER_ADMIN_VALUE, xUserRawValue, StringComparison.OrdinalIgnoreCase);

            if (isAdminUser) return Task.FromResult(UserKind.Admin);

            return Task.FromResult(UserKind.User);
        }

        return Task.FromResult(UserKind.Unknown);
    }
}
