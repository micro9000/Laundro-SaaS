using Laundro.Core.Constants;
using NodaTime;
using System.Globalization;

namespace Laundro.Core.Domain.Shared.Models;
public class YearMonthDay
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }

    public static YearMonthDay FromLocalDate(LocalDate from)
    {
        return new YearMonthDay { Year = from.Year, Month = from.Month, Day = from.Day };
    }

    public static YearMonthDay FromDateOnly(DateOnly from)
    {
        return new YearMonthDay { Year = from.Year, Month = from.Month, Day = from.Day };
    }

    public static YearMonthDay FromDateTime(DateTime from)
    {
        var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(from, DateTimeKind.Utc));
        var result = instant.InZone(DateTimeZoneProviders.Tzdb[BusinessConstants.TimezoneId]).LocalDateTime.Date;
        return FromLocalDate(result);
    }

    public LocalDate ToLocalDate()
    {
        return new LocalDate(Year, Month, Day);
    }

    public DateTime ToDateTimeUtc()
    {
        var local = ToLocalDate().AtStartOfDayInZone(DateTimeZoneProviders.Tzdb[BusinessConstants.TimezoneId]);
        return local.ToDateTimeUtc();
    }

    public YearMonthDay AddDays(int days)
    {
        var value = ToLocalDate();
        value = value.PlusDays(days);
        return YearMonthDay.FromLocalDate(value);
    }

    public override string ToString()
    {
        var dateOnly = new DateOnly(Year, Month, Day);
        return dateOnly.ToString();
    }

    public string ToString(string format)
    {
        var dateOnly = new DateOnly(Year, Month, Day);
        return dateOnly.ToString(format, DateTimeFormatInfo.InvariantInfo);
    }
}
