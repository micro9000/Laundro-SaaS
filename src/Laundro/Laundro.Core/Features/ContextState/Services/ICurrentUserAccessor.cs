using Laundro.Core.Features.ContextState.Models;

namespace Laundro.Core.Features.ContextState.Services;
public interface ICurrentUserAccessor
{
    UserContext? GetCurrentUser();
}
