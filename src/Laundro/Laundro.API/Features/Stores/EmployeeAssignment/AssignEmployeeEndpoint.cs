using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.API.Features.Stores.CreateStore;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.EmployeeAssignment;

internal class AssignEmployeeEndpoint : Endpoint<AssignEmployeeRequest>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly ILogger<CreateStoreEndpoint> _logger;

    public AssignEmployeeEndpoint(
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
        Post("assign-employee");
        Group<StoreGroup>();
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(AssignEmployeeRequest request, CancellationToken ct)
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
            if (selectedUser == null) {
                AddError("Selected employee not found");
                ThrowIfAnyErrors();
            }
            if (selectedRole == null)
            {
                AddError("Selected role not found");
                ThrowIfAnyErrors();
            }

            // Validate if the selected employee/user is already assigned to the same store, with either same role or different
            var existingAssignment = await _dbContext.StoreUsers
                .Include(ss => ss.Role)
                .FirstOrDefaultAsync(ss => ss.UserId == selectedUser!.Id && ss.StoreId == selectedStore!.Id);

            if (existingAssignment is not null)
            {
                var existingRole = existingAssignment.Role;
                AddError($"Employee '{selectedUser?.Name}' is already assigned to '{selectedStore?.Name}' with the role of '{existingRole?.Name}'");
                ThrowIfAnyErrors();
            }
            else
            {
                _dbContext.StoreUsers.Add(new StoreUser
                {
                    StoreId = selectedStore?.Id,
                    RoleId = selectedRole?.Id,
                    UserId = selectedUser?.Id,
                    IsActive = true
                });

                await _dbContext.SaveChangesAsync();

            }
        }
        catch(Exception ex)
        {
            AddError("Unable to assign employee due to internal server error");
            _logger.LogError(ex, ex.Message);
            throw;
        }
        await this.SendStatusCode(201);
    }
}

internal class AssignEmployeeRequest
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public int StoreId { get; set; }
}
