using Laundro.Core.Features.UserContextState.Models;

namespace Laundro.Core.Features.UserContextState.Services;
public interface ICurrentUserAccessor
{
    UserContext? GetCurrentUser();
}
