using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;

public interface ITenantInfoCaching
{
    Task<int?> GetCachedTenantId(int userId);
}

public class TenantInfoCaching : BaseCacheService<int?>, ITenantInfoCaching
{
    private readonly IClaimsRepository _claimsRepository;

    public TenantInfoCaching(ICache cache, IClaimsRepository claimsRepository) : base(cache)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<int?> GetCachedTenantId(int userId)
    {
        var tenantId = await Fetch($"tenantId-{userId}", async e =>
        {
            return await _claimsRepository.GetTenantId(userId);
        });
        return tenantId;
    }

}
