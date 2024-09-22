using Microsoft.Extensions.Caching.Distributed;

namespace Laundro.API.Authentication;

public class UserStateNullCache : IUserStateCache
{
    public async Task<UserState> Fetch(string userEmail, Func<DistributedCacheEntryOptions, Task<UserState>> userContextFactory)
    {
        return await userContextFactory(new DistributedCacheEntryOptions());
    }

    public async Task<UserState> Refresh(string userEmail, Func<DistributedCacheEntryOptions, Task<UserState>> userContextFactory)
    {
        return await Fetch(userEmail, userContextFactory);
    }

    public void Invalidate(string userEmail)
    {
        // NoOp
    }

}
