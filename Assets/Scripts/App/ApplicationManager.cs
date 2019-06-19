using System;
using App;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.UI;
using TapsellSDK;
using TheGame;
using TheGame.Arcade;


public class ApplicationManager : MonoBehaviour
{
    private ViewManager view;

    /*** Menus and Panels ***/
    [Header("Menu Objects")] public ChaptersHandler chaptersHandler;
    public GameObject levelsPanel;
    public GameObject adPanel;

    [Header("Canvases")] public GameObject gameCanvasObject;
    public GameObject mainMenuCanvas;

    [Header("UI Objects")] public Text[] coins;
    public Text[] allGems;
    
    internal static ApplicationManager instance;

    private ApplicationState state;

    public FullPageHorizontalScrollSnap chapterScroller;

    private EnergyHandler energyHandler;



    
    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
    }


    internal void Init()
    {
        view = ViewManager.instance;
        energyHandler = EnergyHandler.instance;

        view.ShowBlackPage();
        view.ShowMainMenu();
        
        chaptersHandler.InitializeChapters();
        chaptersHandler.UpdateChaptersLockState();
        chapterScroller.Init();

        AudioManager.instance.PlayNewMusic(ResourceManager.GetMainMenuMusic());
        
        UpdateCoins();
        UpdateGems();

        chaptersHandler.UpdateChaptersLockState();

        GameAnalytics.Initialize();

        Tapsell.initialize("bjnpopendfnitrefsliijjmdfcebmrberrfnqrcjlthaefiloekpabokjlqbhmglhlhkng");
        Tapsell.setRewardListener(AdReward);

        /*Test*
        Level test = new Level(0, 0, 0, "علی", "اصغر", 0);
        test.SetDynamicFlags(DynamicsFlag.DF_FULL);
        view.LevelClicked(test);
        return;
        /***/
        
        // First Play
        if (!PlayerPrefs.HasKey("first_enter_done"))
        {
            FirstPlay();
            PlayerPrefs.SetInt("first_enter_done", 1);
            PlayerPrefs.Save();
        }

        // Ad Time
        if (PlayerPrefs.HasKey("adWatched"))
        {
            adTimeRemained = SecondsToEndAdWait(PlayerPrefs.GetString("adWatched"));
        }
    }

    
    
    
    #region First Play

    private bool firstPlay = false;
    private void FirstPlay()
    {
        firstPlay = true;
        PlayerPrefs.SetInt("arcade_high_score", 0);
        PlayerPrefs.Save();
        
        energyHandler.Init();
    }

    public void IntroEnded()
    {
        view.IntroEnded();
        firstPlay = false;
    }

    
    /*
    private void GetInstalledApps()
    {
#if UNITY_ANDROID
        AndroidJavaClass pluginClass = new AndroidJavaClass("android.content.pm.PackageManager");
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

        int flag = pluginClass.GetStatic<int>("GET_META_DATA");

        AndroidJavaObject[] arrayOfAppInfo = packageManager.Call<AndroidJavaObject[]>("getInstalledApplications", flag);

        foreach (var appInfo in arrayOfAppInfo)
        {
            PopupHandler.ShowDebug(appInfo.Get<string>("appComponentFactory"));
        }
#endif
    }
    */
    #endregion
    
    
    
    
    #region Coins and Gems
    public void UpdateCoins()
    {
        int coin = DatabaseManager.instance.GetCoins();

        foreach (Text t in coins)
        {
            t.text = coin + "";
        }
    }

    public void UpdateGems()
    {
        GameProgression progress = GameManager.instance.progress;

        for (int c = 0; c < GameManager.instance.chapters.Length; c++)
        {
            int chapG = GetChapterGems(c);
            int levelCount = GameManager.instance.chapters[c].levels.Length;

            chaptersHandler.UpdateChapterGems(c, chapG, levelCount * 3);
        }


        foreach (Text g in allGems)
        {
            g.text = GetAllGemCount() + "";
        }
    }

    private int GetChapterGems(int id)
    {
        GameProgression progress = GameManager.instance.progress;

        int chapG = 0;
        for (int l = 0; l < GameManager.instance.chapters[id].levels.Length; l++)
        {
            if (progress.GetLevelProgress(id, l) != null)
            {
                chapG += progress.GetLevelProgress(id, l).gemTaken;
            }
        }

        return chapG;
    }

    internal int GetAllGemCount()
    {
        return DatabaseManager.instance.GetGems();
    }
    #endregion
    
    
    
    
    #region Advertisement

    [Header("Ad Panels")] public GameObject adLoading;
    public GameObject adNotAvailable;

    public void RequestAd()
    {
        if (adTimeRemained > 0)
        {
            view.AdTimeRemained();
            return;
        }

        view.ShowAdPanel();
    }

    private long SecondsToEndAdWait(string fileTime)
    {
        long now = DateTime.UtcNow.ToFileTimeUtc() / 10000000;
        return (15 * 60) - (now - long.Parse(fileTime));
    }

    public void ShowTabligh()
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

    public void ShowChapterEndAd()
    {
        adLoading.SetActive(true);
        Tapsell.requestAd("5cadc244ac03560001fcab38", false,
            (TapsellAd result) =>
            {
                Tapsell.showAd(result, new TapsellShowOptions());
                adLoading.SetActive(false);
            }, (string zoneId) => { adLoading.SetActive(false); },
            (TapsellError error) => { adLoading.SetActive(false); }
            , (string zoneId) => { adLoading.SetActive(false); }
            , (TapsellAd result) => { adLoading.SetActive(false); });
    }

    private string ad_zone_winDoublePrize = "5cd16a3ddae9a60001f48aec";
    private string ad_zone_mainMenu = "5c6c233aa3455800014d5f40";

    private void AdReward(TapsellAdFinishedResult result)
    {
        if (result.rewarded && result.completed)
        {
            if (result.zoneId == ad_zone_winDoublePrize)
            {
                GameManager.instance.DoublePrizeReward();
            }
            else if (result.zoneId == ad_zone_mainMenu)
                GameManager.instance.AddCoins(30);
        }
    }

    #endregion

    
    

    public void MainMenu_PlayClicked()
    {
        if (firstPlay)
        {
            view.ShowIntro();
        }
        else
        {
            view.MainMenuToChapters();
        }
    }


    internal void RunLevel(Level level)
    {
        GameManager.instance.PlayLevel(level);
        view.ShowGame();
    }

    public void RunArcade()
    {
        ArcadeManager.instance.PlayArcade("تست", 80);
        view.ShowArcade();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public Text adTimeRemainedText;

    private float adTimeRemained;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            view.Escape();
        }

        adTimeRemained -= Time.deltaTime;
        if (adTimeRemained < 0)
            adTimeRemained = 0;

        if (adTimeRemained > 0)
        {
            adTimeRemainedText.transform.parent.gameObject.SetActive(true);
            adTimeRemainedText.text = Utilities.GetTimeFormat(adTimeRemained);
        }
        else
        {
            adTimeRemainedText.transform.parent.gameObject.SetActive(false);
        }
    }


    private void HideAll()
    {
        TutorialHandler.instance.ResetAll();

        adPanel.SetActive(false);
        adLoading.SetActive(false);
        adNotAvailable.SetActive(false);

        ChestHandler.instance.chest.SetActive(false);
    }


    internal void Game_ExitToMenu(int chapter)
    {
        view.GameToMenu();
        AudioManager.instance.PlayNewMusic(AudioManager.instance.GetChapterMusic(chapter));
        chaptersHandler.UpdateChaptersLockState();
    }



    public void Telegram()
    {
        Application.OpenURL("https://t.me/Qolenj_Studio");
    }

    public void Instagram()
    {
        Application.OpenURL("https://www.instagram.com/qolenj_studio/");
    }

    public void Twitter()
    {
        Application.OpenURL("https://twitter.com/Qolenj_Studio");
    }

    public void Site()
    {
        Application.OpenURL("http://qolenj.ir/");
    }

    public bool IsInGame()
    {
        return state == ApplicationState.INGAME;
    }

    // TODO: ViewManager

    #region Developer

    [Header(("Developer Options"))] public GameObject developerPanel;
    private int count = 0;

    public void DeveloperMode()
    {
        count += 1;
        if (count == 14)
        {
            developerPanel.SetActive(true);
        }
    }

    public void DEV_AddCoins()
    {
        GameManager.instance.AddCoins(100);
    }

    public void DEV_AddGems()
    {
        GameManager.instance.AddGems(10);
    }

    public void DEV_UnlockAllChapters()
    {
        chaptersHandler.GetComponent<ChaptersHandler>().UnlockAll();
    }

    public void DEV_FirstPlay()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("developer", 1373);
        PlayerPrefs.Save();
        DatabaseManager.instance.ResetProgress();
        UpdateGems();
        UpdateCoins();
        chaptersHandler.UpdateChaptersLockState();
    }

    public void DEV_SolveLevel()
    {
        GameManager.instance.Solve();
    }

    public void DEV_ToggleMenu(Toggle t)
    {
        if (t.isOn)
        {
            developerPanel.GetComponent<Animator>().SetTrigger("show");
        }
        else
        {
            developerPanel.GetComponent<Animator>().SetTrigger("hide");
        }
    }

    #endregion
}

enum ApplicationState
{
    INGAME,
    FIRSTTUT,
}