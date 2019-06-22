using System;
using App;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.UI;
using TapsellSDK;
using TheGame;
using TheGame.Arcade;
using UnityEditor;


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

    private KeyHandler _keyHandler;

    private GameProgression progress;


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
        _keyHandler = KeyHandler.instance;


        _keyHandler.Init();

        if (isFirstPlay())
            FirstPlay();
        view.ShowBlackPage();
        view.ShowMainMenu();

        if (!PlayerPrefs.HasKey("last_level_unlocked"))
        {
            PlayerPrefs.SetInt("last_level_unlocked", 1);
            PlayerPrefs.Save();
        }

        chaptersHandler.InitializeChapters();
        chaptersHandler.UpdateChaptersLockState();
        chapterScroller.Init();

        progress = DatabaseManager.instance.GetProgressData();

        AudioManager.instance.PlayNewMusic(ResourceManager.GetMainMenuMusic());

        UpdateCoins();
        UpdateGems();

        chaptersHandler.UpdateChaptersLockState();

        GameAnalytics.Initialize();

        AdHandler.instance.Init();
    }


    #region First Play

    private bool isFirstPlay()
    {
        return !PlayerPrefs.HasKey("first_enter_done");
    }

    private bool firstPlay = false;

    private void FirstPlay()
    {
        firstPlay = true;
        PlayerPrefs.SetInt("first_enter_done", 1);
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("arcade_high_score", 0);
        PlayerPrefs.Save();

        _keyHandler.FirstEnter();
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


    public void AddGems(int gem)
    {
        if (gem == 0)
            return;

        DatabaseManager.instance.AddOrRemoveGems(gem);
        UpdateGems();
    }

    private int GetChapterGems(int id)
    {
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

    #endregion


    #region Progress

    public LevelProgression GetLevelProgress(Level lvl)
    {
        return progress.GetLevelProgress(lvl);
    }

    public LevelProgression GetLevelProgress(int chapterId, int levelId)
    {
        return progress.GetLevelProgress(chapterId, levelId);
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


    internal void LevelStartRequest(Level lvl)
    {
        if (lvl.id == progress.GetLastLevel().id && lvl.chapterId == progress.GetLastLevel().chapterId)
            if (PlayerPrefs.GetInt("last_level_unlocked") == 0)
            {
                try
                {
                    _keyHandler.UseEnergy();
                }
                catch (NoKeyException e)
                {
                    view.ShowNoKeyPanel();
                    throw e;
                }

                PlayerPrefs.SetInt("last_level_unlocked", 1);
                PlayerPrefs.Save();
            }
    }

    internal void LevelSolved(Level lvl)
    {
        if (lvl.id != progress.GetLastLevel().id || lvl.chapterId != progress.GetLastLevel().chapterId)
            return;

        PlayerPrefs.SetInt("last_level_unlocked", 0);
        PlayerPrefs.Save();
    }

    internal void RunLevel(Level level)
    {
        view.ShowGame();
        GameManager.instance.PlayLevel(level);
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
        AddGems(10);
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


    public void UpdateLevelProgress(Level lvl, LevelProgression lp)
    {
        if (progress.GetLevelProgress(lp.chapterid, lp.levelid) == null)
        {
            AddGems(lp.gemTaken);
            DatabaseManager.instance.UpdateLevelProgress(lp, false);
        }
        else if (lp.gemTaken > progress.GetLevelProgress(lvl).gemTaken)
        {
            AddGems(lp.gemTaken - progress.GetLevelProgress(lvl).gemTaken);
            DatabaseManager.instance.UpdateLevelProgress(lp, true);
        }

        progress.UpdateLevelProgress(lp);
    }
}

enum ApplicationState
{
    INGAME,
    FIRSTTUT,
}