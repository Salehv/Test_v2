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
    public GameObject adLoading;
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
            PopupHandler.ShowDebug(string.Format("برای نمایش تبلیغ {0} دیگر منتظر بمانید",
                Utilities.GetTimeFormat(coinAdWaitTime - secondPassed)));
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

    private void ShowLoading()
    {
        adLoading.SetActive(true);
    }

    private void HideLoading()
    {
        adLoading.SetActive(false);
    }


    private void ShowCoinAd()
    {
        ShowLoading();
        Tapsell.requestAd(
            ad_zone_coin,
            false,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire,
            Open,
            Close);
    }

    private void Close(TapsellAd obj)
    {
        HideLoading();
    }

    private void Open(TapsellAd obj)
    {
        HideLoading();
    }

    public void ShowPrizeAd()
    {
        ShowLoading();
        Tapsell.requestAd(
            ad_zone_winDoublePrize,
            false,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire);
    }

    public void ShowKeyAd()
    {
        ShowLoading();
        Tapsell.requestAd(
            ad_zone_key,
            false,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire);
    }


    #region Tapsell Actions

    private void AdAvailable(TapsellAd result)
    {
        HideLoading();
        TapsellAd ad = result;
        TapsellShowOptions options = new TapsellShowOptions();
        Tapsell.showAd(ad, options);
    }

    private void AdNotAvailable(string zoneId)
    {
        HideLoading();
        Debug.LogError($"[AdHandler] Ad Not Available for {zoneId}");
        PopupHandler.ShowDebug("در حال حاضر تبلیغی برای نمایش وجود نداره.");
    }

    private void AdError(TapsellError error)
    {
        HideLoading();
        Debug.LogError($"[AdHandler] Ad Error! {error.error}");
        PopupHandler.ShowDebug("نمایش تبلیغ از سمت نمایش دهنده با مشکل مواجه شد");
    }

    private void AdNetworkNotAvailable(string zoneId)
    {
        HideLoading();
        Debug.LogError($"[AdHandler] No Internet! {zoneId}");
        PopupHandler.ShowDebug("ارتباط اینترنت برقرار نشد لطفا اتصال خودتون رو بررسی کنید.");
    }

    private void AdExpire(TapsellAd result)
    {
        HideLoading();
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
                PopupHandler.ShowDebug("تعداد سکه های دریافت شده دو برابر شد!");
            }
            else if (result.zoneId == ad_zone_coin)
            {
                GameManager.instance.AddCoins(30);
                PopupHandler.ShowDebug("30 سکه دریافت شد!");
            }

            else if (result.zoneId == ad_zone_key)
            {
                KeyHandler.instance.AddKeys(3);
                PopupHandler.ShowDebug("3 عدد کلید دریافت شد!");
            }
        }
    }
}