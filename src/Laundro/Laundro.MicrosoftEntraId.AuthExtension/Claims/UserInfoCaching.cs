using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using Laundro.MicrosoftEntraId.AuthExtension.Models;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;

public interface IUserInfoCaching
{
    Task<UserInfo?> GetCachedUserInfo(string email);
}

public class UserInfoCaching : BaseCacheService<UserInfo>, IUserInfoCaching
{
    private readonly IClaimsRepository _claimsRepository;

    public UserInfoCaching(ICache cache, IClaimsRepository claimsRepository) : base(cache)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<UserInfo?> GetCachedUserInfo(string email)
    {
        var userInfo = await Fetch(email, async e =>
        {
            var userInfo = await _claimsRepository.GetUserInfo(email);
            return userInfo;
        });
        return userInfo;
    }
}