using Laundro.Core.Domain.Entities;

namespace Laundro.Core.BusinessRequirementsValidators.TenantsRequirementsAndValidators;
public class UserCanOnlyCreateAndOwnTenantValidator : IBusinessRequirementValidator<Tenant>
{
    private readonly IEnumerable<IBusinessRequirementHandler<UserCanOnlyCreateAndOwnOneTenantRequirement, Tenant>> _validators;

    public UserCanOnlyCreateAndOwnTenantValidator
        (IEnumerable<IBusinessRequirementHandler<UserCanOnlyCreateAndOwnOneTenantRequirement, Tenant>> validators)
    {
        _validators = validators;
    }

    public async Task<ValidatorResponse> Validate(Tenant entity)
    {
        var responses = new List<ValidatorResponse>();
        foreach(var validator in _validators)
        {
            var res = await validator.IsSatisfied(entity);
            responses.Add(res);
        }

        var errors = responses.SelectMany(r => r.ErrorMessages).ToList();

        return new ValidatorResponse
        {
            ErrorMessages = errors
        };
    }
}
