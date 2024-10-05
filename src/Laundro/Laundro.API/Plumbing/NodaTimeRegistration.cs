using NodaTime;

namespace Laundro.API.Plumbing;

public static class NodaTimeRegistration
{
    public static void AddCustomNodaTimeClock(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(_ => SystemClock.Instance);
    }
}
