using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.Lookups;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Employees.Register;

internal class RegisterUserEndpoint : Endpoint<RegisterUserRequest, RegisterUserResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IRoleLookup _roleLookup;
    private readonly IClockService _clock;
    private readonly ILogger<RegisterUserEndpoint> _logger;

    public RegisterUserEndpoint(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        IRoleLookup roleLookup,
        IClockService clock,
        ILogger<RegisterUserEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _roleLookup = roleLookup;
        _clock = clock;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("api/employee/register");
        Policies(PolicyName.IsTenantOwner);
    }

    // Important Note: Users added by the tenant owner are called Employees
    public override async Task HandleAsync(RegisterUserRequest request, CancellationToken c)
    {
        var currentUser = _currentUserAccessor.GetCurrentUser();
        var tenantId = currentUser?.Tenant?.Id;

        var tenantEmployeeRole = await _roleLookup.TenantEmployee();

        var user = _dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefault(u => u.CreatedInTenantId == tenantId && u.Email == request.Email);

        if (user != null)
        {
            if (user.IsActive)
            {
                AddError($"An account with the email {user.Email} already exists");
                _logger.LogError("Tenant owner is trying to add new user with the same employee email");
                ThrowIfAnyErrors();
            }

            user.Name = request.Name;
            user.RoleId = tenantEmployeeRole!.Id;
            if (!user.IsActive)
            {
                user.IsActive = true;
            }
        }
        else
        {
            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                CreatedAt = _clock.Now,
                RoleId = tenantEmployeeRole!.Id,
                CreatedInTenantId = tenantId
            };
            _dbContext.Users.Add(newUser);
            user = newUser;
        }
        await _dbContext.SaveChangesAsync();

        await SendAsync(new()
        {
            Employee = user
        });
    }
}
