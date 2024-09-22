using Laundro.Core.Authentication;
using Microsoft.Extensions.Caching.Distributed;

namespace Laundro.API.Authentication;

public sealed class UserState
{
    public enum UserStateInDb
    {
        NoRecord = 0,
        Inactive = 1,
        Active = 2
    }

    public UserStateInDb DbState { get; set; } = UserStateInDb.NoRecord;

    public UserContext? UserContext { get; set; }
}

public interface IUserStateCache
{
    Task<UserState> Fetch(string userEmail, Func<DistributedCacheEntryOptions, Task<UserState>> userContextFactory);
    Task<UserState> Refresh(string userEmail, Func<DistributedCacheEntryOptions, Task<UserState>> userContextFactory);
    void Invalidate(string userEmail);
}
