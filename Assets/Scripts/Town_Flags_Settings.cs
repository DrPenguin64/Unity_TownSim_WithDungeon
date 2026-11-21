using System;
using Unity.VisualScripting;
using UnityEngine;


public static class Town_Flags_Settings
{
    private static TimeSpan ingame_timeOffset; // from computer's 'now'
    public enum Season { Fall, Winter, Spring, Summer };

    // Flags

    // Adjust offset
    public static void SetGameClock(DateTime correctNow)
    {
        ingame_timeOffset = correctNow - DateTime.Now;
    }

    public static void SetGameClock_Offset(TimeSpan offset)
    {
        ingame_timeOffset = TimeSpan.Zero;
    }

    public static DateTime GetCurrentDateTime_InGame()
    {
        return DateTime.Now + ingame_timeOffset;
    }
    
    public static Season Town_CurrentSeason()
    { // jan=1
        if (GetCurrentDateTime_InGame().Date.Month >= 9 && GetCurrentDateTime_InGame().Date.Month <= 11) return Season.Fall;
        if (GetCurrentDateTime_InGame().Date.Month == 12 || (GetCurrentDateTime_InGame().Date.Month >= 1 && GetCurrentDateTime_InGame().Date.Month <= 2) ) return Season.Winter;
        if (GetCurrentDateTime_InGame().Date.Month >= 3 && GetCurrentDateTime_InGame().Date.Month <= 5) return Season.Spring;
        if (GetCurrentDateTime_InGame().Date.Month >= 6 && GetCurrentDateTime_InGame().Date.Month <= 8) return Season.Summer;
        else return Season.Summer; // not accessible code
    }

    public static void SetTown_Appearance_BasedOn_Season()
    {
        
    }
}
