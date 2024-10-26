using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;

namespace Laundro.API.Features.Stores.CreateStore;

internal class CreateStoreEndpoint : Endpoint<CreateStoreRequest, CreateStoreResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IUserStoresRepository _userStoresRepository;
    private readonly ILogger<CreateStoreEndpoint> _logger;

    public CreateStoreEndpoint(
        ICurrentUserAccessor currentUserAccessor, 
        LaundroDbContext dbContext,
        IClockService clock,
        IUserStoresRepository userStoresRepository,
        ILogger<CreateStoreEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _clock = clock;
        _userStoresRepository = userStoresRepository;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("api/store/create");
        Policies(PolicyName.CanCreateUpdateRetrieveAllStore);
    }

    public override async Task HandleAsync(CreateStoreRequest request, CancellationToken c)
    {
        try
        {
            var currentUser = _currentUserAccessor.GetCurrentUser();
            var tenantId = currentUser?.Tenant?.Id;

            if (tenantId == null)
            {
                AddError("Unable to proceed creating your store due to internal server error");
                _logger.LogError("Unable to create new store due to missing tenant id in the User Context {@UserContext}", currentUser);
            }
            ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point

            var newStore = new Store
            {
                Name = request.Name,
                CreatedAt = _clock.Now,
                TenantId = (int)tenantId!
            };

            _dbContext.Stores.Add(newStore);
            await _dbContext.SaveChangesAsync();

            await _userStoresRepository.RefreshAndGetCachedStoresByTenant(currentUser!.UserId);

            await SendAsync(new()
            {
                Store = newStore
            });
        }
        catch (Exception ex)
        {
            AddError("Unable to fetch all stores due to internal server error");
            _logger.LogError(ex, ex.Message);
            throw;
        }

        ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point
    }
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
    public string? Location { get; set; }
}

internal sealed class CreateStoreResponse
{
    public Store? Store { get; set; }
}