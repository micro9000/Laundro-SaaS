namespace Laundro.API.Authorization;

public static class PolicyName
{
    public const string IsTenantOwner = "IsTenantOwner";
    public const string CanCreateTenant = "CanCreateTenant";
    public const string CanCreateUpdateRetrieveAllStore = "CanCreateUpdateRetrieveAllStore";
}
