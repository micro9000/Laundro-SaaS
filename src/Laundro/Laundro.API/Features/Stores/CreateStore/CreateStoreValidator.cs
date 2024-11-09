using FastEndpoints;

namespace Laundro.API.Features.Stores.CreateStore;

internal class CreateStoreValidator : Validator<CreateStoreRequest>
{
    public CreateStoreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Store Name is required")
            .MinimumLength(3).WithMessage("Your Store name is too short!");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Store Location is required")
            .MinimumLength(3).WithMessage("Your Store Location is too short!");

        RuleFor(x => x.StoreImages)
            .Must(imgs => imgs != null && imgs.Count <= 4)
            .WithMessage("Maximum store images is 4");
    }
}
