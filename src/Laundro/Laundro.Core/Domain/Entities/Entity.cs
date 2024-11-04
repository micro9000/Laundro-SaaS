using NodaTime;

namespace Laundro.Core.Domain.Entities;
public abstract class Entity : ISoftDeletable
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public Instant CreatedAt { get; set; }
    public Instant? DeActivatedOn { get; set; }
}
