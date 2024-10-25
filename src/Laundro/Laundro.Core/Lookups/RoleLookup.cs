using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Laundro.Core.Lookups;
public interface IRoleLookup
{
    Task<Role?> NewUser();
    Task<Role?> TenantOwner();
    Task<Role?> TenantEmployee();
    Task<Role?> StoreManager();
    Task<Role?> StoreStaff();
}

public class RoleLookup : IRoleLookup
{
    private readonly LaundroDbContext _context;
    private readonly ICache _cache;
    private readonly ILogger<RoleLookup> _logger;

    public RoleLookup(LaundroDbContext context, ICache cache, ILogger<RoleLookup> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Role?> NewUser()
    {
        return await GetRole(nameof(Roles.new_user));
    }

    public async Task<Role?> TenantOwner()
    {
        return await GetRole(nameof(Roles.tenant_owner));
    }

    public async Task<Role?> TenantEmployee()
    {
        return await GetRole(nameof(Roles.tenant_employee));
    }

    public async Task<Role?> StoreManager()
    {
        return await GetRole(nameof(Roles.store_manager));
    }

    public async Task<Role?> StoreStaff()
    {
        return await GetRole(nameof(Roles.store_staff));
    }

    private async Task<Role?> GetRole(string role)
    {
        try
        {
            var roles = await GetRoleDb();
            return roles?.SingleOrDefault(r => r.SystemKey == role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Role [{role}] record in database is missing");
            throw;
        }
    }

    private async Task<IEnumerable<Role>?> GetRoleDb()
    {
        var roles = await _cache.GetOrCreateAsync("Laundro.User.Roles", async c =>
        {
            c.SlidingExpiration = TimeSpan.FromMinutes(15);
            return await _context.Roles.AsNoTracking().ToListAsync();
        });

        return roles;
    }
}
