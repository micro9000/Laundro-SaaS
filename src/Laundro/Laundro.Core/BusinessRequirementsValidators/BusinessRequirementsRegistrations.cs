using Laundro.Core.BusinessRequirementsValidators.TenantsRequirementsAndValidators;
using Microsoft.Extensions.DependencyInjection;

namespace Laundro.Core.BusinessRequirementsValidators;
public static class BusinessRequirementsRegistrations
{
    public static IServiceCollection AddBusinessRequirementsValidators(this IServiceCollection services)
    {
        services.AddTenantsRequirementsAndValidators();

        return services;
    }
}
