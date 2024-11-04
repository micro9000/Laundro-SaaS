using FastEndpoints;

namespace Laundro.API.Features.Stores.UpdateStore;

internal class UpdateStoreValidator : Validator<UpdateStoreRequest>
{
    public UpdateStoreValidator()
    {
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Invalid store id");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Store Name is required")
            .MinimumLength(3).WithMessage("Your Store name is too short!");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Store Location is required")
            .MinimumLength(3).WithMessage("Your Store Location is too short!");
    }
}
