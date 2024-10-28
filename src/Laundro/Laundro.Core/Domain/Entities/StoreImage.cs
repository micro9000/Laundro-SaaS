namespace Laundro.Core.Domain.Entities;
public class StoreImage : Entity
{
    public int StoreId { get; set; }
    public string? Url { get; set; }
    public string? ContentType { get; set; }
}
