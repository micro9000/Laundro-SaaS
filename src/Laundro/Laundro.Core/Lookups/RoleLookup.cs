using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Laundro.Core.Lookups;
public interface IRoleLookup
{
    Task<Role?> TenantOwner();
    Task<Role?> StoreManager();
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

    public async Task<Role?> TenantOwner()
    {
        var roles = await GetRoleDb();
        return roles?.SingleOrDefault(r => r.SystemKey == nameof(Roles.tenant_owner));
    }
    
    public async Task<Role?> StoreManager()
    {
        var roles = await GetRoleDb();
        return roles?.SingleOrDefault(r => r.SystemKey == nameof(Roles.store_manager));
    }

    public async Task<Role?> StoreStaff()
    {
        var roles = await GetRoleDb();
        return roles?.SingleOrDefault(r => r.SystemKey == nameof(Roles.store_staff));
    }

    private async Task<IEnumerable<Role>?> GetRoleDb()
    {
        var roles = await _memoryCache.GetOrCreateAsync("Laundro.User.Roles", async c =>
        {
            c.SlidingExpiration = TimeSpan.FromMinutes(15);
            return await _context.Roles.ToListAsync();
        });

        return roles;
    }
}
