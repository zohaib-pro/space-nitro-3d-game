using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.Rendering.DebugUI;

public static class PrefManager
{

    public static int GetSelectedShip()
    {
        return PlayerPrefs.GetInt(Const.KEY_SHIP_SELECTION, 0);
    }

    public static void SetSelectedShip(int shipIdx)
    {
        PlayerPrefs.SetInt(Const.KEY_SHIP_SELECTION, shipIdx);
        PlayerPrefs.Save();
    }
    public static int GetPlayerCoins()
    {
        return PlayerPrefs.GetInt(Const.KEY_COINS, 0);
    }

    public static void AddPlayerCoins(int newCoinsCount)
    {
        PlayerPrefs.SetInt(Const.KEY_COINS, newCoinsCount + GetPlayerCoins());
        PlayerPrefs.Save();
    }

    public static void DeductPlayerCoins(int coinsAmount)
    {
        PlayerPrefs.SetInt(Const.KEY_COINS, GetPlayerCoins() - coinsAmount);
        PlayerPrefs.Save();
    }

    public static bool isShipOwned(int shipId)
    {
        //fist ship is always owned
        return shipId == 0? true : PlayerPrefs.GetInt(Const.KEY_SHIP_OWN + shipId, 0) != 0;
    }

    public static void SetShipOwned(int shipId)
    {
        PlayerPrefs.SetInt(Const.KEY_SHIP_OWN + shipId, 1);
        PlayerPrefs.Save();
    }

    public static string GetControlType()
    {
        return PlayerPrefs.GetString(Const.KEY_CONTROL_TYPE, Const.CONTROL_TILT);
    }

    public static void SetControlType(string controlType)
    {
        PlayerPrefs.SetString(Const.KEY_CONTROL_TYPE, controlType);
        PlayerPrefs.Save();
    }
    
    public static int CheckAndSetHighScore(int newScore)
    {

        int highScore = GetHighScore();
        if (newScore > highScore)
        {
            PlayerPrefs.SetInt(Const.KEY_HIGH_SCORE, newScore);
            PlayerPrefs.Save();
            highScore = newScore;
        }
        // also returns the high score
        return highScore;
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(Const.KEY_HIGH_SCORE, 0);
    }

    public static bool IsMusicOn()
    {
        return PlayerPrefs.GetInt(Const.KEY_IS_MUSIC, 1) == 1; //by default music on
    }

    public static void SetIsMusicOn(bool value)
    {
        PlayerPrefs.SetInt(Const.KEY_IS_MUSIC, value? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool IsSfxOn()
    {
        return PlayerPrefs.GetInt(Const.KEY_IS_SFX, 1) == 1; //by default sfx on
    }

    public static void SetIsSfxOn(bool value)
    {
        PlayerPrefs.SetInt(Const.KEY_IS_SFX, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool IsTutorialCompleted()
    {
        return PlayerPrefs.GetInt(Const.KEY_TUTORIAL_DONE, 0) == 1;
    }

    public static void SetTutorialCompleted()
    {
        PlayerPrefs.SetInt(Const.KEY_TUTORIAL_DONE, 1);
        PlayerPrefs.Save();
    }

    public static float GetLastAdWatchTime()
    {
        return PlayerPrefs.GetFloat(Const.KEY_AD_WATCH_TIME, Utils.Hours() - 48);
    }

    public static void SetLastAdWatchTime()
    {
        PlayerPrefs.SetFloat(Const.KEY_AD_WATCH_TIME, Utils.Hours());
        PlayerPrefs.Save();
    }

    public static bool CanWatchAd()
    {


        // Calculate the time in milliseconds since the Unix epoch
        float nowTime = Utils.Hours();
        float lastTime = GetLastAdWatchTime();
        Debug.Log("Time diff: "+ (nowTime - lastTime));
        return nowTime - lastTime >= 12; //can earn one time per day
    }
}

