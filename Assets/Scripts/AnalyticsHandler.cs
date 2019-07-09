using GameAnalyticsSDK;
using TheGame;
using UnityEngine;

public class AnalyticsHandler : MonoBehaviour
{
    private void Start()
    {
        /*Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });*/
    }

    #region Intro

    public static void Intro_Started()
    {
        GameAnalytics.NewDesignEvent("Intro:01_Started");
//        FirebaseAnalytics.LogEvent("Intro:01_Started");
    }

    public static void Intro_FirstCharacterChanged()
    {
        GameAnalytics.NewDesignEvent("Intro:02_1stChanged");
//        FirebaseAnalytics.LogEvent("Intro:02_1stChanged");
    }

    public static void Intro_SecondCharacterChanged()
    {
        GameAnalytics.NewDesignEvent("Intro:03_2ndChanged");
//        FirebaseAnalytics.LogEvent("Intro:03_2ndChanged");
    }

    public static void Intro_Finished(int time)
    {
        GameAnalytics.NewDesignEvent("Intro:04_Finished", time);
//        FirebaseAnalytics.LogEvent("Intro:04_Finished");
    }

    #endregion


    #region Achivements

    public static void GemsCollection_10(int time)
    {
    }

    public static void GemsCollection_50(int time)
    {
    }

    public static void GemsCollection_100(int time)
    {
    }

    public static void GemsCollection_500(int time)
    {
    }

    #endregion


    #region Progress

    internal static void Tutorial_Completed02()
    {
        if (PlayerPrefs.HasKey("Tutorial_2_Completed"))
            return;

        GameAnalytics.NewDesignEvent("Tutorial:Completed02");
//        FirebaseAnalytics.LogEvent("Tutorial:Completed02");
    }

    internal static void Tutorial_Completed03()
    {
        if (PlayerPrefs.HasKey("Tutorial_3_Completed"))
            return;

        GameAnalytics.NewDesignEvent("Tutorial:Completed03");
//        FirebaseAnalytics.LogEvent("Tutorial:Completed03");
    }

    internal static void Tutorial_Completed04()
    {
        if (PlayerPrefs.HasKey("Tutorial_4_Completed"))
            return;

        GameAnalytics.NewDesignEvent("Tutorial:Completed04");
//        FirebaseAnalytics.LogEvent("Tutorial:Completed04");
    }

    internal static void Tutorial_Completed05()
    {
        if (PlayerPrefs.HasKey("Tutorial_5_Completed"))
            return;

        GameAnalytics.NewDesignEvent("Tutorial:Completed05");
//        FirebaseAnalytics.LogEvent("Tutorial:Completed05");
    }

    internal static void Chapter_Finished(int chapter, int time)
    {
        if (PlayerPrefs.HasKey("Chapter_" + string.Format("{0:00}", chapter) + "_Completed"))
            return;

        GameAnalytics.NewDesignEvent(string.Format("Chapter:Completed{0:00}", chapter));
//        FirebaseAnalytics.LogEvent(string.Format("Chapter:Completed{0:00}", chapter));

        GameAnalytics.NewDesignEvent(string.Format("TotalCoin:Chapter{0:00}", chapter),
            DatabaseManager.instance.GetCoins());

//        FirebaseAnalytics.LogEvent(string.Format("TotalCoin:Chapter{0:00}", chapter), "coin",
//            DatabaseManager.instance.GetCoins());

        // TODO: Add Chapter start and end realtimer
    }

    internal static void Level_Started(int chapter, int level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start,
            string.Format("Chapter{0:00}", chapter),
            string.Format("Level{0:00}", level));
    }

    internal static void Level_Completed(int chapter, int level, int gem, int coin, int time)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete,
            string.Format("Chapter{0:00}", chapter),
            string.Format("Level{0:00}", level));

        GemCollected(gem, "LVLCMP", "LEVEL_COMPLETE_GEM");
        CoinCollected(coin, "LVLCMP", "LEVEL_COMPLETE_COIN");

        GameAnalytics.NewDesignEvent(string.Format("LevelTime_{0:00}-{1:00}", chapter, level), time);
    }

    internal static void ArcadeStarted()
    {
        GameAnalytics.NewDesignEvent("ArcadeStarted");
    }

    internal static void ArcadeLeaved()
    {
        GameAnalytics.NewDesignEvent("ArcadeLeaved");
    }

    internal static void ArcadeFinished(int score)
    {
        GameAnalytics.NewDesignEvent("ArcadeFinished", score);
    }

    #endregion


    #region Resources

    private static void GemCollected(int count, string type, string id)
    {
        if (count > 0)
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "GEM", count, type, id);
    }

    internal static void GemUsed(int count)
    {
        // TODO: Add chapter usage
    }

    private static void CoinCollected(int count, string type, string id)
    {
        if (count > 0)
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "COIN", count, type, id);
    }

    private static void CoinUsed(int count, string type, string id)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "COIN", count, type, id);
    }

    #endregion


    #region Business

    internal static void ItemBought(IABItemType item, int price, int coin)
    {
        GameAnalytics.NewBusinessEvent("IRR", price, "COIN_PACK", string.Format("Coin_{0}", coin), "SHOP");
        CoinCollected(coin, "SHOP", string.Format("Coin_{0}", coin));

        GameAnalytics.NewDesignEvent("ItemBought");
    }

    #endregion


    #region Helpers

    internal static void ChestOpened(int coinAmount, int gemAmount)
    {
        CoinCollected(coinAmount, "CHEST", "CHEST_COIN");
        GemCollected(gemAmount, "CHEST", "CHEST_GEM");
    }

    internal static void HintUsed(int chapterId, int levelId, HintType type)
    {
        switch (type)
        {
            case HintType.SIMILAR:
                GameAnalytics.NewDesignEvent(string.Format("HINT_SIMILAR:CHAPTER{0:00}:LEVEL{1:00}", chapterId + 1,
                    levelId + 1));
                CoinUsed(GameManager.instance.similarHintCost, "HINT", "SIMILAR");
                break;

            case HintType.WAY:
                GameAnalytics.NewDesignEvent(string.Format("HINT_WAY:CHAPTER{0:00}:LEVEL{1:00}", chapterId + 1,
                    levelId + 1));
                CoinUsed(GameManager.instance.showWayHintCost, "HINT", "WAY");
                break;
        }
    }

    public static void EnteredShop()
    {
        GameAnalytics.NewDesignEvent("EnteredShop");
    }

    internal static void PrizeUsed(int chapterId, int levelId, int coin)
    {
        GameAnalytics.NewDesignEvent(string.Format("PRIZE:Chapter{0:00}:Level{0:00}", chapterId + 1, levelId + 1),
            coin);
    }

    internal static void AdWatched()
    {
        GameAnalytics.NewDesignEvent("AD");
    }

    #endregion
}