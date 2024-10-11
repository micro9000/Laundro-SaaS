using Laundro.Core.Features.UserContextState.Repositories;

namespace Laundro.API.Plumbing;

public static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddUserAccountStateRepositories();

        return services;
    }
}
