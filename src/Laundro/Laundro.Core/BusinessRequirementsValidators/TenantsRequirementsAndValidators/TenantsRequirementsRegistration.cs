using Laundro.Core.BusinessRequirementsValidators.TenantsRequirementsAndValidators.Create;
using Laundro.Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Laundro.Core.BusinessRequirementsValidators.TenantsRequirementsAndValidators;
public static class TenantsRequirementsRegistration
{
    public static IServiceCollection AddTenantsRequirements(this IServiceCollection services)
    {
        services.AddScoped<
            IBusinessRequirementHandler<UserCanOnlyOwnOneTenantRequirement, Tenant>,
            IsUserCanCreateMultipleTenantHandler>();

        return services;
    }
}
