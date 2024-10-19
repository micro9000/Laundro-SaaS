using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Domain.Entities;

namespace Laundro.API.Features.Stores.CreateStore;

internal class CreateStoreEndpoint : Endpoint<CreateStoreRequest, CreateStoreResponse>
{
    public CreateStoreEndpoint()
    {
        
    }

    public override void Configure()
    {
        Post("api/tenant/create");
        Policies(PolicyName.CanCreateStore);
    }

    public override async Task HandleAsync(CreateStoreRequest request, CancellationToken c)
    { }
}

internal class CreateStoreValidator : Validator<CreateStoreRequest>
{
    public CreateStoreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Store Name is required")
            .MinimumLength(3).WithMessage("Your Store name is too short!");
    }
}

internal sealed class CreateStoreRequest
{
    public string? Name { get; set; }
}

internal sealed class CreateStoreResponse
{
    public Store? Store { get; set; }
}