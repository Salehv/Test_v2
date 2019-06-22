using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdHandler : MonoBehaviour
{
    /*
    public void ShowCoinAd()
    {
        if (PlayerPrefs.HasKey("adWatched"))
        {
            string s = PlayerPrefs.GetString("adWatched");
            long t = SecondsToEndAdWait(s);
            if (t > 0)
            {
                print(t + " Remained!");
                // toast.showToastOnUiThread("");
                // TODO: Show remaining time

                return;
            }
        }

        adLoading.SetActive(true);
        Tapsell.requestAd("5c6c233aa3455800014d5f40", false,
            (TapsellAd result) =>
            {
                // onAdAvailable
                // Debug.Log("Action: onAdAvailable");
                TapsellAd ad = result; // store this to show the ad later
                Tapsell.showAd(ad, new TapsellShowOptions());
                adLoading.SetActive(false);
                PlayerPrefs.SetString("adWatched", "" + DateTime.UtcNow.ToFileTimeUtc() / 10000000);
                PlayerPrefs.Save();
                adTimeRemained = 15 * 60;
                view.Escape();
                AnalyticsHandler.AdWatched();
            },
            (string zoneId) =>
            {
                // onNoAdAvailable
                Debug.Log("No Ad Available");
                adLoading.SetActive(false);
                adNotAvailable.SetActive(true);
            },
            (TapsellError error) =>
            {
                // onError
                adLoading.SetActive(false);
                Debug.Log(error.error);
            },
            (string zoneId) =>
            {
                // onNoNetwork
                adLoading.SetActive(false);
                adNotAvailable.SetActive(true);
            },
            (TapsellAd result) =>
            {
                // onExpiring
                Debug.Log("Expiring");
                adLoading.SetActive(false);
                // this ad is expired, you must download a new ad for this zone
            }
        );
    }
    
    public void ShowPrizeAd(){}
    
    public void ShowKeyAd(){}

    
    
    private void AdNotAvailable()
    {
        
    }
    */
}