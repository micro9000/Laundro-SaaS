using NodaTime;

namespace Laundro.Core.Domain.Entities;
public interface ISoftDeletable
{
    public bool IsActive { get; set; }
    public Instant? DeActivatedOn { get; set; }
}
