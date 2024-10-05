using Laundro.Core.Authentication.UserAccountCacheRepository;
using Laundro.Core.Data;
using Laundro.Core.Domain.Models;
using Laundro.Core.Lookups;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;

namespace Laundro.Core.Authentication;
public class UserAccountStateRepository
{
    private readonly IClockService _clock;
    private readonly LaundroDbContext _dbContext;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IUserTenantRepository _userTenantRepository;
    private readonly IUserStoresRepository _userStoresRepository;
    private readonly IRoleLookup _roleLookup;

    public UserAccountStateRepository(
        IClockService clock,
        LaundroDbContext dbContext,
        IUserInfoRepository userInfoRepository,
        IUserTenantRepository userTenantRepository,
        IUserStoresRepository userStoresRepository,
        IRoleLookup roleLookup
    )
    {
        _clock = clock;
        _dbContext = dbContext;
        _userInfoRepository = userInfoRepository;
        _userTenantRepository = userTenantRepository;
        _userStoresRepository = userStoresRepository;
        _roleLookup = roleLookup;
    }

    private async Task<UserAccountState> GetCurrentUserAccountState(string userEmail, string userName)
    {
        var newUserAccountState = new UserAccountState();

        var newUserRole = _roleLookup.NewUser();
        var tenantOwnerRole = _roleLookup.TenantOwner();
        var storeManagerRole = _roleLookup.StoreManager();
        var storeStaff = _roleLookup.StoreStaff();

        var user = await _userInfoRepository.GetCachedUserInfo(userEmail);
        if (user is null) 
        {
            var newUser = new User
            {
                Email = userEmail,
                Name = userName,
                RoleId = newUserRole.Id,
                CreatedAt = _clock.DateTimeUtc
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();
            newUserAccountState.DbState = UserAccountState.UserStateInDb.Active;
            return newUserAccountState;
        }
        else
        {
            var tenant = _userTenantRepository.GetCachedTenantByOwner(user.Id);

        }

        return newUserAccountState;
    }
}
