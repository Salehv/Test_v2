using System;
using System.Collections;
using System.Collections.Generic;
using TheGame;
using TheGame.Arcade;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class ViewManager : MonoBehaviour
    {
        internal static ViewManager instance;
        private AudioManager audio;

        // Triggers
        private static readonly int TRIG_TO_CHAPTERS = Animator.StringToHash("to_chapters");
        private static readonly int TRIG_CHAPTER_TO_LEVELS = Animator.StringToHash("chapter_to_levels");
        private static readonly int TRIG_SHOW = Animator.StringToHash("show");
        private static readonly int TRIG_HIDE = Animator.StringToHash("hide");
        private static readonly int TRIG_CHAPTERS_TO_MAIN = Animator.StringToHash("to_main");
        private static readonly int TRIG_LEVELS_TO_CHAPTER = Animator.StringToHash("levels_to_chapter");

        private bool escapable = true;
        public ViewState state;

        #region Init

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(this);
                return;
            }

            instance = this;
            levelsHandler.Init();
        }

        private void Start()
        {
            state = ViewState.MAIN_MENU;
            audio = AudioManager.instance;
        }

        #endregion

        #region Menu

        [Header("Menu")] public GameObject menuCanvas;
        public GameObject menuView;
        public GameObject chaptersView;
        public GameObject settings;
        public Animator[] menuTransitions;
        public FullPageHorizontalScrollSnap chapterScroller;

        [Header("Music Toggles")] public Toggle[] musics;
        [Header("SFX Toggles")] public Toggle[] sfxs;


        private void HideAllMenuViews()
        {
            menuView.SetActive(false);
            chaptersView.SetActive(false);
        }

        internal void ShowMainMenu()
        {
            HideAllFrontPanels();

            menuCanvas.SetActive(true);
            menuView.SetActive(true);
            chaptersView.SetActive(true);

            state = ViewState.MAIN_MENU;
        }


        public void MainMenuToChapters(int lastChapter)
        {
            foreach (Animator a in menuTransitions)
            {
                a.SetTrigger(TRIG_TO_CHAPTERS);
            }

            chapterScroller.SetPage(lastChapter);

            audio.PlayNewSfx(SFX.GAME_UNDO);
            audio.PlayNewMusic(ResourceManager.GetChaptersMusic());
            state = ViewState.CHAPTERS;
        }


        public void SettingsToggled(Toggle t)
        {
            audio.PlayNewSfx(SFX.UI_BUTTON_PRESSED);
            settings.GetComponent<Animator>().SetTrigger(t.isOn ? "open" : "close");
        }


        internal void UpdateMusicState(bool musicOn, bool sfxOn)
        {
            foreach (Toggle t in musics)
                t.isOn = !musicOn;

            foreach (Toggle t in sfxs)
                t.isOn = !sfxOn;
        }

        public void ShowIntro(bool clicked = false)
        {
            menuCanvas.SetActive(false);

            gameCanvas.SetActive(true);
            gameView.SetActive(false);
            arcadeView.SetActive(false);
            introView.SetActive(true);

            TutorialHandler.instance.PlayTutorial_01();
            if (clicked)
                audio.PlayNewSfx(SFX.UI_BUTTON_PRESSED);
            audio.PlayNewMusic(ResourceManager.GetIntroMusic());

            state = ViewState.INTRO;
        }

        public void IntroEnded()
        {
            ShowBlackPage(true);
            AudioManager.instance.PlayNewMusic(ResourceManager.GetMainMenuMusic());
            state = ViewState.MAIN_MENU;
        }

        #endregion

        #region Levels

        [Header("Levels")] public GameObject levelsCanvas;
        public LevelPanelHandler levelsHandler;
        public Image levelsBackground;
        public Text levelsChapterName;
        public Animator[] levelsTransitions;


        public void ChaptersToLevels(int chapterId)
        {
            audio.PlayNewSfx(SFX.UI_WHOOSH);

            levelsCanvas.SetActive(true);
            levelsHandler.SetToChapter(chapterId);

            levelsBackground.sprite = ResourceManager.GetChapterBlurredBackground(chapterId);

            foreach (Animator a in levelsTransitions)
            {
                a.SetTrigger(TRIG_CHAPTER_TO_LEVELS);
            }

            levelsChapterName.text = GameManager.instance.chapters[chapterId].name;

            state = ViewState.LEVELS;

            if (chapterId == 1)
            {
                if (!PlayerPrefs.HasKey("keyTutorialShown"))
                {
                    ShowKeyTutorial();
                    PlayerPrefs.SetInt("keyTutorialShown", 1);
                    PlayerPrefs.Save();
                    return;
                }
            }
        }


        public void LevelClicked(Level level)
        {
            audio.PlayNewSfx(SFX.UI_BUTTON_PRESSED);
            try
            {
                ApplicationManager.instance.LevelStartRequest(level);
            }
            catch (NoKeyException e)
            {
                return;
            }


            menuCanvas.SetActive(false);
            frontCanvas.SetActive(true);
            transition.SetActive(true);
            selectedChapter = level.chapterId;
            TransitionHandler.instance.StartTransition(level);
        }

        #endregion


        #region Game

        [Header("Game")] public GameObject gameCanvas;
        public GameObject gameView;
        public GameObject arcadeView;
        public GameObject introView;
        public GameViewManager gameViewManager;

        private void HideAllGamePanels()
        {
            gameCanvas.SetActive(false);
            gameView.SetActive(false);
            arcadeView.SetActive(false);
            introView.SetActive(false);
        }

        internal void ShowGame()
        {
            HideAllGamePanels();
            gameCanvas.SetActive(true);
            gameView.SetActive(true);
            gameViewManager.ShowGame();
            state = ViewState.IN_GAME;
        }

        public void GameToMenu()
        {
            Escape();
            ShowBlackPage(true);
        }

        public void ShowArcade(bool clicked = false)
        {
            blackPageDestination = "arcade";
            if (clicked)
                audio.PlayNewSfx(SFX.UI_BUTTON_PRESSED);
            audio.PlayNewMusic(ResourceManager.GetArcadeMusic());
            ShowBlackPage(true);
        }

        public void EndArcade()
        {
            blackPageDestination = "main";
            audio.PlayNewMusic(ResourceManager.GetMainMenuMusic());
            ShowBlackPage(true);
        }

        #endregion


        #region Front

        [Header("Front")] public GameObject frontCanvas;
        public Panel aboutUsPanel;
        public Panel shopPanel;
        public Panel adPanel;
        public Panel questionForm;
        public Panel noKeyPanel;
        public Panel keyShopPanel;
        public Panel keyTutorialPanel;
        public GameObject transition;

        private void HideAllFrontPanels()
        {
            transition.SetActive(false);
            panelHandler.HideAll();
        }

        public void ShowAboutUs()
        {
            frontCanvas.SetActive(true);
            ShowPanel(aboutUsPanel);
        }

        public void ShowShop(bool clicked = false)
        {
            frontCanvas.SetActive(true);
            if (clicked)
                audio.PlayNewSfx(SFX.UI_BUTTON_PRESSED);
            ShowPanel(shopPanel);

            // ANALYTICS
            AnalyticsHandler.EnteredShop();
        }

        public void AdTimeRemained()
        {
            PopupHandler.ShowDebug("برای مشاهده تبلیغ باید صبر کنید.");
        }

        public void ShowAdPanel()
        {
            ShowPanel(adPanel);
        }

        public void ShowQuestionForm()
        {
            ShowPanel(questionForm);
        }

        public void ShowNoKeyPanel()
        {
            ShowPanel(noKeyPanel);
        }

        public void ShowKeyShop()
        {
            ShowPanel(keyShopPanel);
        }

        private void ShowKeyTutorial()
        {
            ShowPanel(keyTutorialPanel);
        }

        #endregion


        #region Helpers

        [Header("Helpers")] public GameObject blackPage;
        public Animator blackPageAnimator;
        public ToastMessage toast;
        private string blackPageDestination;

        internal void ShowBlackPage(bool hide = false)
        {
            frontCanvas.SetActive(true);
            blackPage.SetActive(true);
            blackPageAnimator.SetTrigger(!hide ? TRIG_SHOW : TRIG_HIDE);
        }


        private void ShowExitToast()
        {
            toast.showToastOnUiThread("برای خروج دوباره دکمه بازگشت را فشار دهید");
            StartCoroutine(NotExited());
        }

        private IEnumerator NotExited()
        {
            yield return new WaitForSeconds(3);
            if (state == ViewState.EXITING)
                state = ViewState.MAIN_MENU;
        }

        internal void TurnOffCanvases()
        {
            menuCanvas.SetActive(false);
            levelsCanvas.SetActive(false);
            gameCanvas.SetActive(false);
            frontCanvas.SetActive(false);
        }

        #endregion


        #region Panel

        [Header("Panels")] public PanelHandler panelHandler;
        private ViewState lastState = ViewState.ONSCREEN;

        internal void ShowPanel(Panel panel)
        {
            audio.PlayNewSfx(SFX.UI_PANEL_IN);
            panelHandler.ShowPanel(panel);

            if (lastState == ViewState.ONSCREEN)
                lastState = state;
            state = ViewState.ONSCREEN;
        }

        internal void SetUnEscapable()
        {
            escapable = false;
        }

        internal void SetEscapable()
        {
            escapable = true;
        }

        #endregion


        private int selectedChapter;

        public void PageBlacked()
        {
            switch (state)
            {
                case ViewState.IN_GAME:
                    gameCanvas.SetActive(false);
                    menuCanvas.SetActive(true);
                    levelsCanvas.SetActive(true);
                    levelsHandler.SetToChapter(selectedChapter);
                    audio.PlayNewMusic(ResourceManager.GetMainMenuMusic());
                    state = ViewState.LEVELS;
                    print("[ViewManager] State is now " + state.ToString());
                    break;

                case ViewState.INTRO:
                    menuCanvas.SetActive(false);
                    gameCanvas.SetActive(true);
                    gameView.SetActive(false);
                    arcadeView.SetActive(false);
                    introView.SetActive(true);

                    TutorialHandler.instance.PlayTutorial_01();
                    print("[ViewManager] State is now " + state.ToString());
                    break;

                case ViewState.MAIN_MENU:
                    if (blackPageDestination == "arcade")
                    {
                        gameCanvas.SetActive(true);
                        arcadeView.SetActive(true);
                        introView.SetActive(false);
                        menuCanvas.SetActive(false);
                        state = ViewState.ARCADE;
                        ApplicationManager.instance.RunArcade();
                        print("[ViewManager] State is now " + state.ToString());
                    }
                    else
                    {
                        gameCanvas.SetActive(false);
                        introView.SetActive(false);
                        menuCanvas.SetActive(true);
                        state = ViewState.MAIN_MENU;
                        print("[ViewManager] State is now " + state.ToString());
                    }

                    break;

                case ViewState.ARCADE:

                    gameCanvas.SetActive(false);
                    menuCanvas.SetActive(true);
                    state = ViewState.MAIN_MENU;
                    print("[ViewManager] State is now " + state.ToString());
                    break;

                case ViewState.ONSCREEN:
                    SetEscapable();
                    Escape();
                    PageBlacked();
                    print("[ViewManager] State is now " + state.ToString());
                    break;
            }
        }

        public void Escape()
        {
            if (!escapable)
            {
                print("[ViewManager] UnEscapable!");
                return;
            }

            print("[ViewManager] Escape from " + state.ToString());

            switch (state)
            {
                case ViewState.ONSCREEN:
                    audio.PlayNewSfx(SFX.UI_PANEL_OUT);
                    panelHandler.HideTopMostPanel();
                    if (!panelHandler.isAnyPanelActive())
                    {
                        state = lastState;
                        lastState = ViewState.ONSCREEN;
                        print("[ViewManager] State is now " + state.ToString());
                    }

                    break;

                case ViewState.MAIN_MENU:
                    ShowExitToast();

                    state = ViewState.EXITING;
                    break;

                case ViewState.CHAPTERS:
                    foreach (Animator a in menuTransitions)
                    {
                        a.SetTrigger(TRIG_CHAPTERS_TO_MAIN);
                    }

                    audio.PlayNewMusic(ResourceManager.GetMainMenuMusic());
                    state = ViewState.MAIN_MENU;
                    print("[ViewManager] State is now " + state.ToString());
                    break;

                case ViewState.LEVELS:
                    chaptersView.SetActive(true);
                    foreach (Animator a in levelsTransitions)
                    {
                        a.SetTrigger(TRIG_LEVELS_TO_CHAPTER);
                    }

                    AudioManager.instance.PlayNewSfx(SFX.UI_WHOOSH_REV);

                    state = ViewState.CHAPTERS;
                    print("[ViewManager] State is now " + state.ToString());
                    break;

                case ViewState.IN_GAME:
                    gameViewManager.Escape();
                    print("[ViewManager] State is now " + state.ToString());
                    break;

                case ViewState.EXITING:
                    Application.Quit();
                    break;

                case ViewState.ARCADE:
                    ArcadeManager.instance.Stop();
                    print("[ViewManager] State is now " + state.ToString());
                    break;

                default: return;
            }
        }

        public void ShowAdRemainingTime()
        {
        }
    }
}

public enum ViewState
{
    MAIN_MENU,
    CHAPTERS,
    LEVELS,
    IN_GAME,
    ONSCREEN,
    AD,
    EXITING,
    INTRO,
    ARCADE
}