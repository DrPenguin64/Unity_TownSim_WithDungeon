using UnityEngine;
using System.Globalization;
using System;
using static Town_Flags_Settings;

public static class DayTime
{
    public static string[] MONTHS = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    public static string GetCurrentTimeStr()
    {
        return GetCurrentDateTime_InGame().ToString("h:mm tt");
    }

    public static string GetDayOfWeek()
    {
        return GetCurrentDateTime_InGame().ToString("ddd", CultureInfo.InvariantCulture);
    }

    public static string GetCurrentMonth()
    {
        return GetCurrentDateTime_InGame().ToString("MMM", CultureInfo.InvariantCulture);
    }

    public static string GetNumDayInMonth()
    {
        return GetCurrentDateTime_InGame().Day.ToString();
    }

    public static string GetCurYear()
    {
        return GetCurrentDateTime_InGame().Year.ToString();
    }

    public static TimeSpan GetDateTimeOffset(DateTime wrongTime, DateTime correctTime)
    {
        return correctTime - wrongTime;
    }
}
