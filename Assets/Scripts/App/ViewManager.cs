using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class ViewManager : MonoBehaviour
    {
        internal static ViewManager instance;

        // Triggers
        private static readonly int TRIG_TO_CHAPTERS = Animator.StringToHash("to_chapters");
        private static readonly int TRIG_PANEL_OUT = Animator.StringToHash("Panel_Out");
        private static readonly int TRIG_CHAPTER_TO_LEVELS = Animator.StringToHash("chapter_to_levels");
        private static readonly int TRIG_SHOW = Animator.StringToHash("show");
        private static readonly int TRIG_CHAPTERS_TO_MAIN = Animator.StringToHash("to_main");
        private static readonly int TRIG_LEVELS_TO_CHAPTER = Animator.StringToHash("levels_to_chapter");


        public ViewState state;

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(this);
                return;
            }

            instance = this;

            onScreenPanels = new Stack<GameObject>();
        }

        private void Start()
        {
            state = ViewState.MAIN_MENU;
        }


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
            levelsHandler.Init(chapterId);
            AudioManager.instance.PlayNewMusic(AudioManager.instance.GetChapterMusic(chapterId));

            state = ViewState.LEVELS;

            levelsBackground.sprite = ResourceManager.instance.GetChapterBluredBackground(chapterId);

            foreach (Animator a in levelsTransitions)
            {
                a.SetTrigger(TRIG_CHAPTER_TO_LEVELS);
            }

            levelsChapterName.text = GameManager.instance.chapters[chapterId].name;
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
        }

        public void GameToMenu()
        {
            throw new System.NotImplementedException();
        }

        public void ShowArcade()
        {
            throw new System.NotImplementedException();
        }

        public void EndArcade()
        {
            throw new System.NotImplementedException();
        }

        #endregion


        #region Front

        [Header("Front")] public GameObject frontCanvas;
        public GameObject aboutUsPanel;
        public GameObject shopPanel;
        public GameObject adPanel;
        public QuestionFormHandler questionForm;
        public GameObject transition;

        private void HideAllFrontPanels()
        {
            transition.SetActive(false);
            aboutUsPanel.SetActive(false);
            shopPanel.SetActive(false);
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
            ShowPanel(questionForm.gameObject);
        }

        #endregion


        #region Helpers

        [Header("Helpers")] public GameObject blackPage;

        public Animator blackPageAnimator;
        public ToastMessage toast;

        internal void ShowBlackPage()
        {
            frontCanvas.SetActive(true);
            blackPage.SetActive(true);
            blackPageAnimator.SetTrigger(TRIG_SHOW);
        }

        public void Escape()
        {
            print("[ViewManager] Escape from " + state.ToString());

            switch (state)
            {
                case ViewState.ONSCREEN:
                    GameObject panel = onScreenPanels.Pop();
                    panel.SetActive(false);

                    if (onScreenPanels.Count == 0)
                    {
                        state = lastState;
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

                    AudioManager.instance.PlayNewMusic(AudioManager.instance.mainThemeMusic);

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

        private ViewState lastState;
        private Stack<GameObject> onScreenPanels;


        public void HidePanel(Animator panel)
        {
            panel.SetTrigger(TRIG_PANEL_OUT);
        }

        private void ShowPanel(GameObject panel)
        {
            onScreenPanels.Push(panel);
            panel.SetActive(true);
            lastState = state;
            state = ViewState.ONSCREEN;
        }

        #endregion
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
    EXITING
}