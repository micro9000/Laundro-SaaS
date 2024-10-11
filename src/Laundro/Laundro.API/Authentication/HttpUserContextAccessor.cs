using Laundro.Core.Features.ContextState.Models;
using Laundro.Core.Features.ContextState.Services;

namespace Laundro.API.Authentication;

public class HttpUserContextAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserContext? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.Items[UserContext.Key] as UserContext;
    }
}
