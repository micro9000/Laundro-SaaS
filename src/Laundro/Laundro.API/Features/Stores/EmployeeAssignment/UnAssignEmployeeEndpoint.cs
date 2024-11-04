using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.API.Features.Stores.CreateStore;
using Laundro.Core.Data;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Laundro.API.Features.Stores.EmployeeAssignment;

internal class UnAssignEmployeeEndpoint : Endpoint<UnAssignEmployeeRequest>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly ILogger<CreateStoreEndpoint> _logger;

    public UnAssignEmployeeEndpoint(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        IClockService clock,
        ILogger<CreateStoreEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _clock = clock;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("unassign-employee");
        Group<StoreGroup>();
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(UnAssignEmployeeRequest request, CancellationToken ct)
    {
        var currentUser = _currentUserAccessor.GetCurrentUser();
        var tenantId = currentUser?.Tenant?.Id;

        try
        {
            var selectedStore = await _dbContext.Stores
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == request.StoreId);
            var selectedUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.CreatedInTenantId == tenantId && u.Id == request.UserId);
            var selectedRole = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Id == request.RoleId);

            if (selectedStore == null)
            {
                AddError("Selected store not found");
                ThrowIfAnyErrors();
            }
            if (selectedUser == null)
            {
                AddError("Selected employee not found");
                ThrowIfAnyErrors();
            }
            if (selectedRole == null)
            {
                AddError("Selected role not found");
                ThrowIfAnyErrors();
            }

            var updatedCount = await _dbContext.StoreUsers
                .Where(ss =>
                    ss.UserId == selectedUser!.Id
                    && ss.StoreId == selectedStore!.Id
                    && ss.RoleId == selectedRole!.Id)
                .ExecuteUpdateAsync(u => 
                    u.SetProperty(ss => ss.IsActive, false)
                    .SetProperty(ss => ss.DeActivatedOn, _clock.Now));

            if (updatedCount > 0) {
                await SendOkAsync(ct);
                return;
            }
        }
        catch (Exception ex)
        {
            AddError("Unable to un-assign employee due to internal server error");
            _logger.LogError(ex, ex.Message);
            throw;
        }
        ThrowIfAnyErrors();
    }
}

internal class UnAssignEmployeeRequest
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public int StoreId { get; set; }
}