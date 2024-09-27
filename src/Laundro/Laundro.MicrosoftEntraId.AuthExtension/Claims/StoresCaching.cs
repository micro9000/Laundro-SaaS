using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using Laundro.MicrosoftEntraId.AuthExtension.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;

public interface IStoresCaching
{
    Task<List<StoreInfo>?> GetCachedStoresByManagerId(int userId);
    Task<List<StoreInfo>?> GetCachedStoresByStaffId(int userId);
}

public class StoresCaching : BaseCacheService<List<StoreInfo>>, IStoresCaching
{
    private readonly IClaimsRepository _claimsRepository;

    public StoresCaching(ICache cache, IClaimsRepository claimsRepository) : base(cache)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<List<StoreInfo>?> GetCachedStoresByManagerId(int userId)
    {
        var stores = await Fetch($"stores-managerId-{userId}", async (e) =>
        {
            var storesDb = await _claimsRepository.GetStoresByManagerId(userId);
            return storesDb.ToList();
        });
        return stores;
    }

    public async Task<List<StoreInfo>?> GetCachedStoresByStaffId(int userId)
    {
        var stores = await Fetch($"stores-staffId-{userId}", async (e) =>
        {
            var storesDb = await _claimsRepository.GetStoresByStaffId(userId);
            return storesDb.ToList();
        });
        return stores;
    }
}
