using FastEndpoints;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;

namespace Laundro.API.Features.Tenants.CreateTenant;

internal class CreateTenantEndpoint : Endpoint<CreateTenantRequest, CreateTenantResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;

    public CreateTenantEndpoint(ICurrentUserAccessor currentUserAccessor, LaundroDbContext dbContext)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("api/tenant/create");
    }

    public override async Task HandleAsync(CreateTenantRequest request, CancellationToken c)
    {

    }

    private async Task<bool> IsCurrentUserIsAllowedToCreateNewTenant(CreateTenantRequest request)
    {
        var userId = _currentUserAccessor.GetCurrentUser()?.UserId;

        if (userId is not null)
        {
            var userActiveTenant = _dbContext.Tenants.FirstOrDefault(t => t.OwnerId == userId);

        }

        return false;
    }
}

internal class CreateTenantValidator : Validator<CreateTenantRequest>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Campany Name is required")
            .MinimumLength(3).WithMessage("Your Campany name is too short!");
    }
}

internal sealed class CreateTenantRequest
{
    public string? CompanyName { get; set; }
}

internal sealed class CreateTenantResponse
{
    public Tenant? Tenant { get; set; }
}
