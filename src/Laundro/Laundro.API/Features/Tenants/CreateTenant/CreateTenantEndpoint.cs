using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.BusinessRequirementsValidators;
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
    private readonly IBusinessRequirementValidator<Tenant> _businessRequirementsValidators;

    public CreateTenantEndpoint(ICurrentUserAccessor currentUserAccessor, 
        LaundroDbContext dbContext,
        IUserTenantRepository userTenantRepository,
        IBusinessRequirementValidator<Tenant> businessRequirementsValidators)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _userTenantRepository = userTenantRepository;
        _businessRequirementsValidators = businessRequirementsValidators;
    }

    public override void Configure()
    {
        Post("api/tenant/create");
        Policies(PolicyName.CanCreateTenant);
    }

    public override async Task HandleAsync(CreateTenantRequest request, CancellationToken c)
    {
        var currentUser = _currentUserAccessor.GetCurrentUser();

        // TODO: Add validation - the user can only have one tenant

        var newTenant = new Tenant
        {
            OwnerId = currentUser!.UserId,
            CompanyName = request.CompanyName
        };

        var validationResponse = await _businessRequirementsValidators.Validate(newTenant);


        _dbContext.Tenants.Add(newTenant);
        await _dbContext.SaveChangesAsync();

        // TODO: try to use FastEndpoints In-Process Event Bus Pattern (Pub/Sub) feature
        // to refresh the cached tenant of the current user
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
