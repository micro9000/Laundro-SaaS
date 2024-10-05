using Laundro.Core.Constants;
using NodaTime;
using NodaTime.TimeZones;

namespace Laundro.Core.NodaTime;
public class ClockService : IClockService
{
    private readonly IClock _clock;

    public DateTimeZone TimeZone { get; private set; }

    public ClockService(IClock clock)
    {
        _clock = clock;

        // NOTE: Get the current users timezone here instead of hard coding it...
        TimeZone = DateTimeZoneProviders.Tzdb[BusinessConstants.TimezoneId];

    }

    public DateTime DateTimeUtc
        => _clock.GetCurrentInstant().ToDateTimeUtc();

    public Instant Now
        => _clock.GetCurrentInstant();

    public LocalDateTime LocalNow
        => Now.InZone(TimeZone).LocalDateTime;

    public Instant? ToInstant(LocalDateTime? local)
        => local?.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

    public LocalDateTime? ToLocal(Instant? instant)
        => instant?.InZone(TimeZone).LocalDateTime;
}