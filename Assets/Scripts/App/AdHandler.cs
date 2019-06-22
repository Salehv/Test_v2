using System;
using System.Collections;
using System.Collections.Generic;
using App;
using TapsellSDK;
using TheGame;
using UnityEngine;
using UnityEngine.AI;

public class AdHandler : MonoBehaviour
{
    internal static AdHandler instance;
    public int coinAdWaitTime;

    private string ad_zone_coin = "5c6c233aa3455800014d5f40";
    private string ad_zone_winDoublePrize = "5cd16a3ddae9a60001f48aec";
    private string ad_zone_key = "5d0dbef3ffe1ef0001e3489a";


    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        Tapsell.initialize("bjnpopendfnitrefsliijjmdfcebmrberrfnqrcjlthaefiloekpabokjlqbhmglhlhkng");
        Tapsell.setRewardListener(AdReward);
    }


    public void RequestCoinAd()
    {
        if (!PlayerPrefs.HasKey("lastAdWatched"))
        {
            ShowCoinAd();
            CoinAdWatched();
            return;
        }


        long lastTimePlayed = long.Parse(PlayerPrefs.GetString("lastAdWatched"));
        if (lastTimePlayed == -1)
        {
            ShowCoinAd();
            CoinAdWatched();
            return;
        }

        long secondPassed = TimeManager.instance.Now() - lastTimePlayed;
        if (secondPassed < coinAdWaitTime)
        {
            ViewManager.instance.ShowAdRemainingTime();
            return;
        }

        ShowCoinAd();
        CoinAdWatched();
    }

    private void CoinAdWatched()
    {
        PlayerPrefs.SetString("lastAdWatched", "" + TimeManager.instance.Now());
        PlayerPrefs.Save();
        AnalyticsHandler.AdWatched();
    }

    private void ShowCoinAd()
    {
        Tapsell.requestAd(
            ad_zone_coin,
            true,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire);
    }

    public void ShowPrizeAd()
    {
        Tapsell.requestAd(
            ad_zone_winDoublePrize,
            true,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire);
    }

    public void ShowKeyAd()
    {
        Tapsell.requestAd(
            ad_zone_key,
            true,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire);
    }


    #region Tapsell Actions

    private void AdAvailable(TapsellAd result)
    {
        TapsellAd ad = result;
        TapsellShowOptions options = new TapsellShowOptions();
        Tapsell.showAd(ad, options);
    }

    private void AdNotAvailable(string zoneId)
    {
        Debug.LogError($"[AdHandler] Ad Not Available for {zoneId}");
    }

    private void AdError(TapsellError error)
    {
        Debug.LogError($"[AdHandler] Ad Error! {error.error}");
    }

    private void AdNetworkNotAvailable(string zoneId)
    {
        Debug.LogError($"[AdHandler] No Internet! {zoneId}");
    }

    private void AdExpire(TapsellAd result)
    {
        Debug.LogError($"[AdHandler] Expired! {result.zoneId}");
    }

    #endregion


    private void AdReward(TapsellAdFinishedResult result)
    {
        if (result.rewarded && result.completed)
        {
            if (result.zoneId == ad_zone_winDoublePrize)
            {
                GameManager.instance.DoublePrizeReward();
            }
            else if (result.zoneId == ad_zone_coin)
                GameManager.instance.AddCoins(30);

            else if (result.zoneId == ad_zone_key)
            {
                KeyHandler.instance.AddKeys(3);
            }
        }
    }
}