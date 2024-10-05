using Laundro.Core.NodaTime;
using NodaTime;

namespace Laundro.API.Plumbing;

public static class NodaTimeRegistration
{
    public static IServiceCollection AddCustomNodaTimeClock(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(_ => SystemClock.Instance);
        services.AddSingleton<IClockService, ClockService>();
        return services;
    }
}
