using NodaTime;

namespace Laundro.Core.NodaTime;
public static class NodaTimeExtensions
{
    public static string ToDateTimeString(this LocalDateTime? local)
    {
        if (local == null)
        {
            return null;
        }

        return local.Value.ToDateTimeString();
    }

    public static string ToDateTimeString(this LocalDateTime local)
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        return local.ToString($"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}", culture);
    }

    public static string ToShortDateString(this LocalDate? local)
    {
        if (local == null)
        {
            return null;
        }

        return local.Value.ToShortDateString();
    }

    public static string ToShortDateString(this LocalDate local)
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        return local.ToString(culture.DateTimeFormat.ShortDatePattern, culture);
    }
}