namespace Laundro.Core.Domain.Entities;
public class Role
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? SystemKey { get; set; }

    public ICollection<StoreUser> StoreUser { get; set; } = [];
}
