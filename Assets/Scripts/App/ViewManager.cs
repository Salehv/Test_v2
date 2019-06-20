using System.Collections;
using System.Collections.Generic;
using TheGame;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class ViewManager : MonoBehaviour
    {
        internal static ViewManager instance;

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
        }
        #endregion

        #region Menu

        [Header("Menu")] public GameObject menuCanvas;
        public GameObject menuView;
        public GameObject chaptersView;
        public GameObject settings;
        public Animator[] menuTransitions;

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


        public void MainMenuToChapters()
        {
            foreach (Animator a in menuTransitions)
            {
                a.SetTrigger(TRIG_TO_CHAPTERS);
            }

            state = ViewState.CHAPTERS;
        }


        public void SettingsToggled(Toggle t)
        {
            settings.GetComponent<Animator>().SetTrigger(t.isOn ? "open" : "close");
        }


        internal void UpdateMusicState(bool musicOn, bool sfxOn)
        {
            foreach (Toggle t in musics)
                t.isOn = !musicOn;

            foreach (Toggle t in sfxs)
                t.isOn = !sfxOn;
        }
        
        public void ShowIntro()
        {
            ShowBlackPage(true);
            AudioManager.instance.PlayNewMusic(ResourceManager.GetInGameMusic(0));
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
            levelsCanvas.SetActive(true);
            levelsHandler.SetToChapter(chapterId);
            AudioManager.instance.PlayNewMusic(AudioManager.instance.GetChapterMusic(chapterId));

            levelsBackground.sprite = ResourceManager.GetChapterBluredBackground(chapterId);

            foreach (Animator a in levelsTransitions)
            {
                a.SetTrigger(TRIG_CHAPTER_TO_LEVELS);
            }

            levelsChapterName.text = GameManager.instance.chapters[chapterId].name;
            
            state = ViewState.LEVELS;
        }

        public void LevelClicked(Level level)
        {
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
            ShowBlackPage(true);
        }

        public void ShowArcade()
        {
            gameCanvas.SetActive(true);
            gameView.SetActive(false);
            arcadeView.SetActive(true);
        }

        public void EndArcade()
        {
            throw new System.NotImplementedException();
        }

        #endregion


        #region Front

        [Header("Front")] public GameObject frontCanvas;
        public Panel aboutUsPanel;
        public Panel shopPanel;
        public Panel adPanel;
        public Panel questionForm;
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

        public void ShowShop()
        {
            frontCanvas.SetActive(true);
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

        #endregion


        #region Helpers

        [Header("Helpers")] public GameObject blackPage;

        public Animator blackPageAnimator;
        public ToastMessage toast;

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

        [Header("Panels")] 
        public PanelHandler panelHandler;
        private ViewState lastState = ViewState.ONSCREEN;
        
        internal void ShowPanel(Panel panel)
        {
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
                    state = ViewState.LEVELS;
                    break;
                
                case ViewState.INTRO:
                    menuCanvas.SetActive(false);
                    
                    gameCanvas.SetActive(true);
                    gameView.SetActive(false);
                    arcadeView.SetActive(false);
                    introView.SetActive(true);
                    
                    TutorialHandler.instance.PlayTutorial_01();
                    break;
                
                case ViewState.MAIN_MENU:
                    gameCanvas.SetActive(false);
                    introView.SetActive(false);
                    menuCanvas.SetActive(true);
                    break;
                case ViewState.ONSCREEN:
                    state = lastState;
                    PageBlacked();
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
                    panelHandler.HideTopMostPanel();
                    if (!panelHandler.isAnyPanelActive())
                    {
                        state = lastState;
                        lastState = ViewState.ONSCREEN;
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

                    state = ViewState.MAIN_MENU;
                    break;

                case ViewState.LEVELS:
                    chaptersView.SetActive(true);
                    foreach (Animator a in levelsTransitions)
                    {
                        a.SetTrigger(TRIG_LEVELS_TO_CHAPTER);
                    }

                    AudioManager.instance.PlayNewMusic(ResourceManager.GetMainMenuMusic());

                    state = ViewState.CHAPTERS;
                    break;

                case ViewState.IN_GAME:
                    gameViewManager.Escape();
                    break;

                case ViewState.EXITING:
                    Application.Quit();
                    break;

                /*
                case ViewState.AD:
                    adPanel.SetActive(false);
                    state = ApplicationState.MAIN_MENU;
                    AudioManager.instance.PlaySFX(2);
                    break;
                case ViewState.QUESTION_FORM:
                    questionForm.gameObject.SetActive(false);
                    state = ApplicationState.MAIN_MENU;
                    break;
                */

                default: return;
            }
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
    INTRO
}