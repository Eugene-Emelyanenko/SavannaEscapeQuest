using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Coins
{
    public readonly static string coinsKey = "Coins";

    public static void SaveCoins(int value)
    {
        PlayerPrefs.SetInt(coinsKey, value);
        PlayerPrefs.Save();
    }

    public static int GetCoins() => PlayerPrefs.GetInt(coinsKey, 1000);
}

public static class Water
{
    public readonly static string waterKey = "Water";

    public static void SaveWater(int value)
    {
        TigerLevel currentLevel = TigerLevels.GetLevel(PlayerPrefs.GetInt("Tiger", 1));
        value = Mathf.Clamp(value, 0, currentLevel.maxFood);
        PlayerPrefs.SetInt(waterKey, value);
        PlayerPrefs.Save();
    }

    public static int GetWater() => PlayerPrefs.GetInt(waterKey, 0);
}

public static class Meat
{
    public readonly static string meatKey = "Meat";

    public static void SaveMeat(int value)
    {
        TigerLevel currentLevel = TigerLevels.GetLevel(PlayerPrefs.GetInt("Tiger", 1));
        value = Mathf.Clamp(value, 0, currentLevel.maxFood);
        PlayerPrefs.SetInt(meatKey, value);
        PlayerPrefs.Save();
    }

    public static int GetMeat() => PlayerPrefs.GetInt(meatKey, 0);
}

