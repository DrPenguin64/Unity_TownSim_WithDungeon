using UnityEngine;
using System;

public static class Player_OnSaveLoad
{
    // "Permanent" data
    public static string playerName;
    public static int playerGender; // 0 = female, 1 = male
    public static DateTime player_Birthday;
    public static int playerEyeNum;
    public static int playerHairNum;

    public static int playerHousePos; // 0 1 2 3 = top right, top left, bottom left, bottom right

    // Outfit
    public static int playerShirtNum;
    public static int playerHatNum;

    // Special event related
    public static bool player_flag_savedLastTime = true;
    public static bool player_flag_isMoveInSequence;



}
