using Laundro.Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Laundro.Core.BusinessRequirementsValidators.TenantsRequirementsAndValidators;
public static class TenantsRequirementsAndValidatorsRegistration
{
    public static IServiceCollection AddTenantsRequirementsAndValidators(this IServiceCollection services)
    {
        services.AddScoped<
            IBusinessRequirementHandler<UserCanOnlyCreateAndOwnOneTenantRequirement, Tenant>,
            IsUserCreatingOrOwningMultipleTenantHandler>();
        services.AddScoped<IBusinessRequirementValidator<Tenant>, UserCanOnlyCreateAndOwnTenantValidator>();

        return services;
    }
}
