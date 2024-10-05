using Laundro.Core.Repository.UserAccountCacheRepository;

namespace Laundro.API.Plumbing;

public static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddUserAccountStateRepositories();

        return services;
    }
}
