using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Features.UserContextState.Services;

namespace Laundro.API.Features.Tenants.CreateTenant;

internal class CreateTenantEndpoint : Endpoint<CreateTenantRequest, CreateTenantResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IUserTenantRepository _userTenantRepository;

    public CreateTenantEndpoint(ICurrentUserAccessor currentUserAccessor, 
        LaundroDbContext dbContext,
        IUserTenantRepository userTenantRepository)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _userTenantRepository = userTenantRepository;
    }

    public override void Configure()
    {
        Post("api/tenant/create");
        Policies(PolicyName.CanCreateTenant);
    }

    public override async Task HandleAsync(CreateTenantRequest request, CancellationToken c)
    {
        var currentUser = _currentUserAccessor.GetCurrentUser();
        var newTenant = new Tenant
        {
            OwnerId = currentUser!.UserId,
            CompanyName = request.CompanyName
        };

        _dbContext.Tenants.Add(newTenant);
        await _dbContext.SaveChangesAsync();

        var userNewTenant = await _userTenantRepository.RefreshAndGetCachedTenantByOwner(currentUser!.UserId);

        await SendAsync(new()
        {
            Tenant = userNewTenant
        });
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
