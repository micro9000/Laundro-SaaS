namespace Laundro.Core.Domain.Entities;
public class Role
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? SystemKey { get; set; }

    // These should not be nullable
    // To avoid this error: CS8620 - Argument cannot be used for parameter due to differences in the nullability of reference types.
    public ICollection<StoreUser> StoreUser { get; set; } = new List<StoreUser>();
}
