using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Employees.GetAll;

internal class GetAllEmployeesEndpoint : EndpointWithoutRequest<GetAllEmployeesResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly ILogger<GetAllEmployeesEndpoint> _logger;

    public GetAllEmployeesEndpoint(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        ILogger<GetAllEmployeesEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("api/employee/getall");
        Policies(PolicyName.IsTenantOwner);
    }

    // I'm not expecting thousands of employees in Laundry business
    // so it is okay to return all employees
    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<User> employees = Enumerable.Empty<User>();

        try
        {
            var tenantId = _currentUserAccessor.GetCurrentUser()?.Tenant?.Id;
            if (tenantId != null)
            {
                employees = await _dbContext.Users
                    .Where(s => s.CreatedInTenantId == tenantId)
                    .ToListAsync(ct);
            }
        }
        catch (Exception ex)
        {
            AddError("Unable to fetch all stores due to internal server error");
            _logger.LogError(ex, ex.Message);
            throw;
        }

        await SendAsync(new GetAllEmployeesResponse
        {
            Employees = employees
        });
    }
}
