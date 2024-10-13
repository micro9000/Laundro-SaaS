using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.BusinessRequirementsValidators;
using Laundro.Core.BusinessRequirementsValidators.TenantsRequirementsAndValidators.Create;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;

namespace Laundro.API.Features.Tenants.CreateTenant;

internal class CreateTenantEndpoint : Endpoint<CreateTenantRequest, CreateTenantResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IUserTenantRepository _userTenantRepository;
    private readonly IClockService _clock;
    private readonly IEnumerable<IBusinessRequirementHandler<UserCanOnlyOwnOneTenantRequirement, Tenant>> _canCreateValidator;

    public CreateTenantEndpoint(ICurrentUserAccessor currentUserAccessor, 
        LaundroDbContext dbContext,
        IUserTenantRepository userTenantRepository,
        IClockService clock,
        IEnumerable<IBusinessRequirementHandler<UserCanOnlyOwnOneTenantRequirement, Tenant>> canCreateValidator)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _userTenantRepository = userTenantRepository;
        _clock = clock;
        _canCreateValidator = canCreateValidator;
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
            CompanyName = request.CompanyName,
            CreatedAt = _clock.Now.ToDateTimeUtc()
        };

        var validationResponse = await CanCreateValidate(newTenant);
        if (!validationResponse.IsSatisfied)
        {
            foreach(var errMsg in validationResponse.ErrorMessages)
            {
                AddError(errMsg);
            }
        }

        ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point

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

    public async Task<ValidatorResponse> CanCreateValidate(Tenant entity)
    {
        var responses = new List<ValidatorResponse>();
        foreach (var validator in _canCreateValidator)
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
