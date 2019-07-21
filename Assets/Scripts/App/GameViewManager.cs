using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPersian.Components;

namespace App
{
    public class GameViewManager : MonoBehaviour
    {
        internal static GameViewManager instance;
        private ViewManager view;
        public GameObject gamePanelsObject;

        [Header("Panels")] [SerializeField] private Panel winPanel;
        [SerializeField] private Panel pausePanel;
        [SerializeField] private Panel hintPanel;
        [SerializeField] private Panel hintSimilarWordsPanel;
        [SerializeField] private GameObject tut01Panel;


        [Header("Game View")] public Image inGameBack;
        public Image inGameFeatures;


        [Header("Objects")] public GameObject acceptedWordPrefab;

        public GameObject stepViewer;
        public Text hintSimilarCostText;
        public Text hintWayCostText;

        [Header("UI Handlers")] public WinPanelHandler winHandler;
        public EndWordHandler endWordHandler;


        private GameViewState state;
        private List<string> words;

        private void Awake()
        {
            instance = this;
            view = ViewManager.instance;
        }


        #region Initialize

        public void ShowGame()
        {
            gamePanelsObject.SetActive(true);
            pausePanel.gameObject.SetActive(false);
            hintPanel.gameObject.SetActive(false);
            hintSimilarWordsPanel.gameObject.SetActive(false);
            winPanel.gameObject.SetActive(false);
            tut01Panel.SetActive(false);
            stepViewer.SetActive(true);
            TutorialHandler.instance.ResetAll();
            state = GameViewState.MAIN_VIEW;
        }


        [Header("Game Intro")] public GameObject btnHint;
        public GameObject btnShuffle;
        public GameObject btnPause;
        public GameObject btnUndo;
        public GameObject endWord;

        public void ShowGameIntro()
        {
            gamePanelsObject.SetActive(true);
            pausePanel.gameObject.SetActive(false);
            hintPanel.gameObject.SetActive(false);
            hintSimilarWordsPanel.gameObject.SetActive(false);
            winPanel.gameObject.SetActive(false);

            tut01Panel.SetActive(true);
            btnHint.SetActive(false);
            btnShuffle.SetActive(false);
            btnPause.SetActive(false);
            btnUndo.SetActive(false);
            endWord.SetActive(false);
            HideStepViewer();

            state = GameViewState.INTRO;
        }


        public void SetInGameGraphics(int chapter)
        {
            inGameBack.sprite = ResourceManager.GetInGameBackground(chapter);
            inGameFeatures.sprite = ResourceManager.GetInGameFeatures(chapter);
        }


        public void SetEndWord(string end)
        {
            endWord.SetActive(true);
            for (int i = 0; i < endWordHandler.contents.childCount; i++)
            {
                Destroy(endWordHandler.contents.GetChild(i).gameObject);
            }

            for (int i = end.Length - 1; i >= 0; i--)
            {
                endWordHandler.CreateLetter(end[i]);
            }
        }

        public void InitStepViewer(int steps)
        {
            StepViewerHandler.instance.gameObject.SetActive(true);
            StepViewerHandler.instance.Init(steps);
        }

        #endregion


        #region WordsView

        [Header("Words View")] public RtlText currentWord;
        public RtlText prevWord;
        public RtlText prevPrevWord;
        public Animator[] wvAnimators;

        public void ClearWordsView()
        {
            currentWord.text = "";
            prevWord.text = "";
            prevPrevWord.text = "";
            words = new List<string>();
        }

        public void AddToWordsView(string word)
        {
            prevPrevWord.text = prevWord.BaseText;
            prevWord.text = currentWord.BaseText;
            currentWord.text = word;
            foreach (var wvAnimator in wvAnimators)
            {
                wvAnimator.SetTrigger("roll_in");
            }

            words.Add(word);

            StepViewerHandler.instance.StepForward();
        }

        public void RemoveLastWord()
        {
            currentWord.text = prevWord.BaseText;
            prevWord.text = prevPrevWord.BaseText;

            try
            {
                prevPrevWord.text = words[words.Count - 4];
            }
            catch (Exception e)
            {
                prevPrevWord.text = "";
            }

            words.RemoveAt(words.Count - 1);

            StepViewerHandler.instance.StepBackward();
        }

        #endregion


        #region Panels

        public void ShowWinPanel(int gem, int coinGain)
        {
            state = GameViewState.WIN;
            winPanel.gameObject.SetActive(true);
            winHandler.Init(gem, coinGain);
        }

        public void ShowHintPanel()
        {
            view.ShowPanel(hintPanel);
        }

        public void ShowSimilarWordsHintPanel()
        {
            view.ShowPanel(hintSimilarWordsPanel);
            view.SetUnEscapable();
        }

        public void SimilarSelected()
        {
            view.SetEscapable();
            view.Escape();
        }

        #endregion


        internal void Escape()
        {
            switch (state)
            {
                case GameViewState.MAIN_VIEW:
                    view.ShowPanel(pausePanel);
                    break;
            }
        }

        public void HideStepViewer()
        {
            stepViewer.SetActive(false);
        }

        public void ShowAllButtons()
        {
            btnHint.SetActive(true);
            btnUndo.SetActive(true);
            btnPause.SetActive(true);
            btnShuffle.SetActive(true);
        }
    }
}

public enum GameViewState
{
    MAIN_VIEW,
    WIN,
    INTRO
}