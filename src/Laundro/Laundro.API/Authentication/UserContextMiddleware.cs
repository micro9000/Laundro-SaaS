using Laundro.Core.Authentication;
using Laundro.Core.Data;
using Laundro.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Web;

namespace Laundro.API.Authentication;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUserStateCache _userStateCache;
    private readonly ICache _cache;
    private readonly ILogger<UserContextMiddleware> _logger;

    public UserContextMiddleware(
        RequestDelegate next,
        IUserStateCache userStateCache,
        ICache cache,
        ILogger<UserContextMiddleware> logger)
    {
        _next = next;
        _userStateCache = userStateCache;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, LaundroDbContext dbContext)
    {
        //var userEmail = httpContext?.User?.Identity?.Name;
        var userEmail = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.PreferredUserName)?.Value;
        if (httpContext != null && !string.IsNullOrWhiteSpace(userEmail))
        {
            var role = await GetUserRoleClaim(dbContext, httpContext);
            var userName = GetNameClaim(httpContext);
            var userAuthId = GetObjectIdClaim(httpContext);

            var currentUserState = await _userStateCache.Fetch(userEmail, async e =>
            {
                var res = await GetUserSateAndContext(userEmail, role, userName, dbContext, e);
                _logger.LogInformation("User {UserId} ({AuthenticationId} retrieved from database not cache",
                    res.UserContext?.UserId, userAuthId);
                return res;
            });

            var roleIsStillSameAsBefore = false;
            if (currentUserState.DbState == UserState.UserStateInDb.Active && currentUserState.UserContext != null)
            {
                roleIsStillSameAsBefore = currentUserState.UserContext.Role?.SystemKey != null &&
                        currentUserState.UserContext.Role.SystemKey.Equals(role?.SystemKey, StringComparison.OrdinalIgnoreCase);

                if (roleIsStillSameAsBefore)
                {
                    SetHttpContext(currentUserState.UserContext, httpContext);
                }
            }

            // If the role has changed, or the user inactive, re-check and construct the context
            if (!roleIsStillSameAsBefore || (role != null && currentUserState.DbState == UserState.UserStateInDb.Inactive))
            {
                // Enables role to be changed by resetting user context cache when role from Auth Extension is not the same as already in cache
                var newUserState = await _userStateCache.Refresh(userEmail, async e =>
                {
                    return await GetUserSateAndContext(userEmail, role, userName, dbContext, e);
                });
                if (newUserState.DbState == UserState.UserStateInDb.Active && newUserState.UserContext != null)
                {
                    SetHttpContext(newUserState.UserContext, httpContext);
                    _logger.LogInformation("User {UserId} ({AuthenticationId} checked - role change successful",
                        newUserState.UserContext.UserId, userAuthId);
                }
            }
        }

        await _next(httpContext!);
    }


    private void SetHttpContext (UserContext currentUserContext, HttpContext httpContext)
    {
        httpContext.Items[UserContext.Key] = currentUserContext;
    }

    private async Task<Role?> GetUserRoleClaim(LaundroDbContext dbContext, HttpContext httpContext)
    {
        var allRoles = await _cache.GetOrCreateAsync($"{nameof(UserContextMiddleware)}-AllRoles",
            async e =>
            {
                try
                {
                    var roles = await dbContext.Roles.AsNoTracking().ToListAsync();
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return roles;
                }
                catch(Exception exception)
                {
                    _logger.LogError(exception, "Unable to retrieve roles from the database");
                    return null;
                }
            }).ConfigureAwait(false);

        if (allRoles == null)
        {
            _logger.LogError("Unable to retrieve roles from the db or cache");
            return null;
        }

        var roleClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Role);
        if (roleClaim == null)
        {
            _logger.LogWarning("Unable to retrieve user role claim");
            return null;
        }

        var role = allRoles.FirstOrDefault(r => r.SystemKey.Equals(roleClaim.Value, StringComparison.OrdinalIgnoreCase));
        return role;
    }

    private async Task<UserState> GetUserSateAndContext
        (string userEmail, Role? role, string userName, LaundroDbContext dbContext, DistributedCacheEntryOptions entry)
    {
        var result = new UserState();
        var user = await GetUser(dbContext, userEmail);
        if (user is null)
        {
            result.DbState = UserState.UserStateInDb.NoRecord;
            if (role == null)
            {
                _logger.LogWarning("Unable to register new user {email} due to missing role claim", userEmail);
                return result;
            }
            // Add user to db if they have been assigned a role and they haven't been added previously
            user = new User { Email = userEmail, Name = userName, RoleId = role.Id, IsActive = true };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            result.DbState = UserState.UserStateInDb.Active;
        }
        else
        {
            await UpdateUserData(user, role, userName, dbContext);
            result.DbState = user.IsActive ? UserState.UserStateInDb.Active : UserState.UserStateInDb.Inactive;
        }

        if (!user.IsActive || role == null)
        {
            return result;
        }

        // Changes to user properties (e.g. role) will take up to an hour to propagate unless explicitly invalidated in cache
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
        entry.SlidingExpiration = TimeSpan.FromMinutes(10);

        var userContext = new UserContext
        {
            UserId = user.Id,
            Email = user.Email,
            Role = new RoleContext { Id = role.Id, SystemKey = role.SystemKey },
        };

        var stores = await dbContext.Stores
            .Include(s => s.StaffAssignments)
            .AsNoTracking()
            .Where(s => s.ManagerId == user.Id || s.StaffAssignments.Any(u => u.UserId == user.Id))
            .ToListAsync();

        foreach (var store in stores) 
        {
            var storeContext = new StoreContext
            {
                StoreId = store.Id,
                ManagerId = store.ManagerId,
                StaffUserIds = store.StaffAssignments.Select(a => a.UserId).ToList()
            };
            userContext.Stores.Add(storeContext);
        }

        result.UserContext = userContext;
        return result;
    }

    private async Task UpdateUserData(User user, Role? role, string userName, LaundroDbContext dbContext)
    {
        var isUserDirty = false;
        switch(user.IsActive)
        {
            case true when role == null:
                // Set user to inactive if they no longer have a valid role
                _logger.LogInformation("Setting user {UserId} to inactive as has no role", user.Id);
                user.IsActive = false;
                isUserDirty =true;
                break;
            case true when user.RoleId != role.Id:
                // Update role if the user is active but their role has changed
                _logger.LogInformation("Update user role {UserId} with role {Role}", user.Id, role.Name);
                user.RoleId = role.Id;
                user.Role = role;
                isUserDirty = true;
                break;
            case false when role != null:
                _logger.LogInformation("Reactiving inactive user {UserId} with role {Role}", user.Id, role.Name);
                user.RoleId = role.Id;
                user.Role = role;
                user.IsActive = true;
                isUserDirty = true;
                break;
            case false when role == null:
                _logger.LogInformation("Inactive user {UserId} not added as has no role", user.Id);
                break;
        }

        // Update persisted name if it does not match the one in the claim
        if (user.Name != userName)
        {
            user.Name = userName;
            isUserDirty = true;
        }
        
        if (isUserDirty)
        {
            var entity = dbContext.Users.Attach(user);
            entity.State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            _logger.LogInformation("User {UserId} updated in database", user.Id);
        }
    }

    private static string? GetNameClaim(HttpContext httpContext) =>
        httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Name)?.Value;

    private static string? GetObjectIdClaim(HttpContext httpContext) =>
        httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ObjectId)?.Value;

    private static async Task<User?> GetUser(LaundroDbContext dbContext, string userEmail)
    {
        return await dbContext.Users.IgnoreQueryFilters()
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == userEmail);
    }
}
