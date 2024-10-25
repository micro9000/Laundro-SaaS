using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.BusinessRequirementsValidators;
using Laundro.Core.BusinessRequirementsValidators.TenantsRequirementsAndValidators.Create;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Laundro.API.Features.Tenants.CreateTenant;

internal class CreateTenantEndpoint : Endpoint<CreateTenantRequest, CreateTenantResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IUserTenantRepository _userTenantRepository;
    private readonly IUserStoresRepository _userStoresRepository;
    private readonly IClockService _clock;
    private readonly IEnumerable<IBusinessRequirementHandler<UserCanOnlyOwnOneTenantRequirement, Tenant>> _canCreateValidator;
    private readonly ILogger<CreateTenantEndpoint> _logger;

    public CreateTenantEndpoint(ICurrentUserAccessor currentUserAccessor, 
        LaundroDbContext dbContext,
        IUserTenantRepository userTenantRepository,
        IUserStoresRepository userStoresRepository,
        IClockService clock,
        IEnumerable<IBusinessRequirementHandler<UserCanOnlyOwnOneTenantRequirement, Tenant>> canCreateValidator,
        ILogger<CreateTenantEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _userTenantRepository = userTenantRepository;
        _userStoresRepository = userStoresRepository;
        _clock = clock;
        _canCreateValidator = canCreateValidator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("api/tenant/create");
        Policies(PolicyName.CanCreateTenant);
    }

    public override async Task HandleAsync(CreateTenantRequest request, CancellationToken c)
    {
        var currentUser = _currentUserAccessor.GetCurrentUser();

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, c))
            {
                try
                {
                    var newTenant = new Tenant
                    {
                        OwnerId = currentUser!.UserId,
                        TenantName = request.TenantName,
                        CompanyAddress = request.CompanyAddress,
                        WebsiteUrl = request.WebsiteUrl,
                        BusinessRegistrationNumber = request.BusinessRegistrationNumber,
                        PrimaryContactName = request.PrimaryContactName,
                        ContactEmail = request.ContactEmail,
                        PhoneNumber = request.PhoneNumber,
                        TenantGuid = Guid.NewGuid(), // TODO: Upgrade to Guid.CreateVersion7() once we upgrade to .NET 9
                        CreatedAt = _clock.Now,
                    };

                    var validationResponse = await CanCreateValidate(newTenant);
                    if (!validationResponse.IsSatisfied)
                    {
                        foreach (var errMsg in validationResponse.ErrorMessages)
                        {
                            AddError(errMsg);
                        }
                    }

                    var initialStore = new Store
                    {
                        Name = request.StoreName,
                        Location = request.StoreLocation,
                        Tenant = newTenant,
                        CreatedAt = _clock.Now,
                    };

                    ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point

                    await _dbContext.Stores.AddAsync(initialStore);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    _logger.LogError(ex, ex.Message);

                    throw;
                }
            }
        });

        // TODO: try to use FastEndpoints In-Process Event Bus Pattern (Pub/Sub) feature
        // to refresh the cached tenant of the current user
        var userNewTenant = await _userTenantRepository.RefreshAndGetCachedTenantByOwner(currentUser!.UserId);
        await _userStoresRepository.RefreshAndGetCachedStoresByUser(currentUser!.UserId);

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
        RuleFor(x => x.TenantName)
            .NotEmpty().WithMessage("Tenant/Campany Name is required")
            .MinimumLength(3).WithMessage("Your Tenant/Campany name is too short!");

        RuleFor(x => x.CompanyAddress)
            .NotEmpty().WithMessage("Campany Address is required")
            .MinimumLength(3).WithMessage("Your Campany Address is too short!");

        RuleFor(x => x.PrimaryContactName)
            .NotEmpty().WithMessage("Primary Contact Name is required")
            .MinimumLength(3).WithMessage("Your Primary Contact Name is too short!");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact Email is required")
            .MinimumLength(3).WithMessage("Your Contact Email is too short!")
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Contact Email is required")
            .MinimumLength(11).MaximumLength(12);

        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Initial Store Name is required")
            .MinimumLength(3).WithMessage("Your Initial Store Name is too short!");

        RuleFor(x => x.StoreLocation)
            .NotEmpty().WithMessage("Initial Store Location is required")
            .MinimumLength(3).WithMessage("Your Initial Store Location is too short!");
    }
}

internal sealed class CreateTenantRequest
{
    // Tenant Info
    public string? TenantName { get; set; }
    public string? CompanyAddress { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? BusinessRegistrationNumber { get; set; }
    public string? PrimaryContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }

    // Store Info
    public string? StoreName { get; set; }
    public string? StoreLocation { get; set; }
}

internal sealed class CreateTenantResponse
{
    public Tenant? Tenant { get; set; }
}
