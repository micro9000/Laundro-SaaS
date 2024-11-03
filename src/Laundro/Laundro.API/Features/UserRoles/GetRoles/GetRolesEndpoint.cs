using FastEndpoints;
using Laundro.API.Features.Stores.GetStores;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.UserRoles.GetRoles;

internal class GetRolesEndpoint : EndpointWithoutRequest<GetRolesResponse>
{
    private readonly LaundroDbContext _dbContext;
    private readonly ILogger<GetStoresEndpoints> _logger;

    public GetRolesEndpoint(LaundroDbContext dbContext,
        ILogger<GetStoresEndpoints> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public override void Configure()
    {
        Get("api/role/getall");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Don't include the StoreUser property

        var roles = await _dbContext.Roles.ToListAsync();
        await SendAsync(new GetRolesResponse
        {
            Roles = roles
        });
    }
}

internal class GetRolesResponse
{
    public IEnumerable<Role>? Roles { get; set; }
}