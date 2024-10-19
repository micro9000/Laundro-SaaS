using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Models;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Lookups;
using Laundro.Core.NodaTime;

namespace Laundro.Core.Features.UserContextState.Services;

public interface IUserAccountStateService
{
    Task<UserAccountState> GetAndUpsertCurrentUserAccountState(string userEmail, string userName);
}

public class UserAccountStateService : IUserAccountStateService
{
    private readonly IClockService _clock;
    private readonly LaundroDbContext _dbContext;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IUserTenantRepository _userTenantRepository;
    private readonly IUserStoresRepository _userStoresRepository;
    private readonly IRoleLookup _roleLookup;

    public UserAccountStateService(
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

    public async Task<UserAccountState> GetAndUpsertCurrentUserAccountState(string userEmail, string userName)
    {
        var newUserAccountState = new UserAccountState();

        var newUserRole = await _roleLookup.NewUser();
        var tenantOwnerRole = await _roleLookup.TenantOwner();
        var tenantEmployeeRole = await _roleLookup.TenantEmployee();

        var user = await _userInfoRepository.GetCachedUserInfo(userEmail);
        if (user is null)
        {
            var newUser = new User
            {
                Email = userEmail,
                Name = userName,
                RoleId = newUserRole!.Id,
                CreatedAt = _clock.DateTimeUtc
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            newUserAccountState.DbState = UserAccountState.UserStateInDb.Active;
            newUserAccountState.UserContext = new UserContext
            {
                UserId = newUser.Id,
                Email = newUser.Email,
                Role = newUserRole
            };
            return newUserAccountState;
        }

        user.Role = null; // Preventing an update query
        var tenant = await _userTenantRepository.GetCachedTenantByOwner(user.Id);
        var storesByUser = await _userStoresRepository.GetCachedStoresByUser(user.Id);

        bool userDetailsHasChanged = false;

        var userContext = new UserContext
        {
            UserId = user.Id,
            Email = user.Email
        };

        if (!user.IsActive)
        {
            userDetailsHasChanged = true;
            user.IsActive = true;
        }

        if (tenant is not null)
        {
            var storesByTenant = await _userStoresRepository.RefreshAndGetCachedStoresByTenant(tenant.Id);

            if (user.RoleId != tenantOwnerRole!.Id)
            {
                userDetailsHasChanged = true;
                user.RoleId = tenantOwnerRole!.Id;
            }

            userContext.Tenant = tenant;
            userContext.Role = tenantOwnerRole;
            userContext.IsTenantOwner = true;
            userContext.Stores = storesByTenant;
        }
        else if (storesByUser is not null && storesByUser.Any())
        {
            // Changing Employee Role is not necessary, as a tenant owner, you choose to assign employee role

            var associatedTenantOfTheStore = storesByUser.First().Tenant;
            userContext.Tenant = associatedTenantOfTheStore;
            userContext.Role = tenantEmployeeRole;
            userContext.Stores = storesByUser;
        }
        else
        {
            userDetailsHasChanged = user.RoleId != newUserRole!.Id;
            user.RoleId = newUserRole!.Id;
            userContext.Role = newUserRole;
        }

        if (userDetailsHasChanged)
        {
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        newUserAccountState.DbState = UserAccountState.UserStateInDb.Active;
        newUserAccountState.UserContext = userContext;
        return newUserAccountState;
    }
}
