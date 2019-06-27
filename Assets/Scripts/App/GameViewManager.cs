using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


        [Header("Game View")] public Image inGameBack;
        public Image inGameFeatures;


        [Header("Objects")] public Transform wordsView;
        public GameObject acceptedWordPrefab;

        public Text hintSimilarCostText;
        public Text hintWayCostText;

        [Header("UI Handlers")] public WinPanelHandler winHandler;
        public EndWordHandler endWordHandler;


        private GameViewState state;

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
            TutorialHandler.instance.ResetAll();
            state = GameViewState.MAIN_VIEW;
        }


        public void SetInGameGraphics(int chapter)
        {
            inGameBack.sprite = ResourceManager.GetInGameBackground(chapter);
            inGameFeatures.sprite = ResourceManager.GetInGameFeatures(chapter);
        }


        public void SetEndWord(string end)
        {
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

        public void ClearWordsView()
        {
            for (int i = 0; i < wordsView.childCount; i++)
                Destroy(wordsView.GetChild(i).gameObject);
        }

        public void AddToWordsView(string word)
        {
            GameObject g = Instantiate(acceptedWordPrefab, wordsView);
            g.GetComponentInChildren<Text>().text = word;

            StepViewerHandler.instance.StepForward();
        }

        public void RemoveLastWord()
        {
            Destroy(wordsView.GetChild(wordsView.childCount - 1).gameObject);

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
            StepViewerHandler.instance.gameObject.SetActive(false);
        }
    }
}

public enum GameViewState
{
    MAIN_VIEW,
    WIN
}