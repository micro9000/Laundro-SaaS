using NodaTime;

namespace Laundro.Core.NodaTime;
public interface IClockService
{
    DateTimeZone TimeZone { get; }

    DateTime DateTimeUtc { get; }
    Instant Now { get; }

    LocalDateTime LocalNow { get; }

    Instant? ToInstant(LocalDateTime? local);

    LocalDateTime? ToLocal(Instant? instant);
}