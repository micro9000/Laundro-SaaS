using NodaTime;

namespace Laundro.Core.Domain.Entities;
public interface ISoftDeletable
{
    public bool IsActive { get; set; }
    public DateTimeOffset? DeActivatedOn { get; set; }
}
