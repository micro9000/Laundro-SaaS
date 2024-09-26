using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Data;

public interface IUserCache
{
    Task<string?> GetUserRole(string email);
}

public class UserCache : BaseCacheService<string>, IUserCache
{
    private readonly IUserRepository _userRepository;

    public UserCache(ICache cache, IUserRepository userRepository) : base(cache)
    {
        _userRepository = userRepository;
    }

    public async Task<string?> GetUserRole(string email)
    {
        var userRoleSystemKey = await Fetch(email, async e =>
        {
            var roleSystemKey = await _userRepository.GetUserRole(email);
            return roleSystemKey;
        });
        return userRoleSystemKey;
    }

}
