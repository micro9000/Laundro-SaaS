using FastEndpoints;

namespace Laundro.API.Features.Stores.CreateStore;

internal class CreateStoreValidator : Validator<CreateStoreRequest>
{
    public CreateStoreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Store Name is required")
            .MinimumLength(3).WithMessage("Your Store name is too short!");
    }
}
