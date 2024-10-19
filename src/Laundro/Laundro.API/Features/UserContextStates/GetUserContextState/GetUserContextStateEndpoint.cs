using FastEndpoints;
using Entities = Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Models;
using Laundro.Core.Features.UserContextState.Services;

namespace Laundro.API.Features.UserContextStates.GetUserContextState;

public sealed class UserContextResponse
{
    public int UserId { get; set; }
    public string? Email { get; set; }
    public string? TenantName { get; set; }
    public string? TenantGuid { get; set; }

    public bool IsTenantOwner { get; set; }
    public string? RoleSystemKey { get; set; }
    public List<Entities.Store>? Stores { get; set; }
}

internal sealed class UserContextMapper : ResponseMapper<UserContextResponse, UserContext>
{
    public override UserContextResponse FromEntity(UserContext e) => new()
    {
        UserId = e.UserId,
        Email = e.Email,
        TenantName = e.Tenant?.TenantName,
        TenantGuid = e.Tenant?.TenantGuid.ToString(),
        RoleSystemKey = e.Role?.SystemKey,
        Stores = e.Stores,
        IsTenantOwner = e.IsTenantOwner
    };
}

internal sealed class GetUserContextStateEndpoint : EndpointWithoutRequest<UserContextResponse, UserContextMapper>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public GetUserContextStateEndpoint(ICurrentUserAccessor currentUserAccessor)
    {
        _currentUserAccessor = currentUserAccessor;
    }
    public override void Configure()
    {
        Get("api/user-context-state");
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var currentUserContext = _currentUserAccessor.GetCurrentUser();
        var respnose = Map.FromEntity(currentUserContext!);
        await SendAsync(respnose);
    }
}