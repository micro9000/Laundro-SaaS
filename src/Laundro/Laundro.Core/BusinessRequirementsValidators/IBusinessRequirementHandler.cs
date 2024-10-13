using Laundro.Core.Domain.Entities;

namespace Laundro.Core.BusinessRequirementsValidators;
public interface IBusinessRequirementHandler<IBusinessRequirement, TEntity>
    where IBusinessRequirement : class
    where TEntity : Entity
{
    Task<ValidatorResponse> IsSatisfied(TEntity entity);
}
