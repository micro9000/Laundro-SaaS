using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Models;
using Laundro.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Laundro.Core.Lookups;
public interface IRoleLookup
{
    Task<Role?> StoreAdminAssistant();
    Task<Role?> StoreOwnerAdmin();
    Task<Role?> StoreStaff();
}

public class RoleLookup : IRoleLookup
{
    private readonly LaundroDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public RoleLookup(LaundroDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public async Task<Role?> StoreOwnerAdmin()
    {
        var roles = await GetRoleDb();
        return roles?.SingleOrDefault(r => r.SystemKey == nameof(Roles.store_owner_admin));
    }

    public async Task<Role?> StoreAdminAssistant()
    {
        var roles = await GetRoleDb();
        return roles?.SingleOrDefault(r => r.SystemKey == nameof(Roles.store_admin_assistant));
    }

    public async Task<Role?> StoreStaff()
    {
        var roles = await GetRoleDb();
        return roles?.SingleOrDefault(r => r.SystemKey == nameof(Roles.store_staff));
    }

    private async Task<IEnumerable<Role>?> GetRoleDb()
    {
        var roles = await _memoryCache.GetOrCreateAsync("Roles", async c =>
        {
            c.SlidingExpiration = TimeSpan.FromMinutes(15);
            return await _context.Roles.ToListAsync();
        });

        return roles;
    }
}
