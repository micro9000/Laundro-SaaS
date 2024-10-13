using Laundro.Core.Domain.Entities;

namespace Laundro.Core.BusinessRequirementsValidators;
public interface IBusinessRequirementValidator<TEntity> where TEntity : Entity
{
    Task<ValidatorResponse> Validate(TEntity entity);
}
