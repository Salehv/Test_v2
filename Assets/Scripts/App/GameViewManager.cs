using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class GameViewManager : MonoBehaviour
    {
        internal static GameViewManager instance;
        public GameObject gamePanelsObject;
    
        [Header("Panels")] 
        public GameObject winPanel;
        public GameObject pausePanel;
        public GameObject hintPanel;
        
    
        [Header("Game View")]
        public Image inGameBack;
        public Image inGameFeatures;


        [Header("Objects")] 
        public Transform wordsView;
        public GameObject acceptedWordPrefab;
    
        public Text hintSimilarCostText;
        public Text hintWayCostText;

        [Header("UI Handlers")]
        public WinPanelHandler winHandler;
        public EndWordHandler endWordHandler;


        private GameViewState state;
        
        private void Awake()
        {
            instance = this;
        }

        
        #region Initialize
        
        public void ShowGame()
        {
            gamePanelsObject.SetActive(true);
            pausePanel.SetActive(false);
            hintPanel.SetActive(false);
            winPanel.SetActive(false);
            onScreenPanels = new Stack<GameObject>();
            TutorialHandler.instance.ResetAll();
            state = GameViewState.MAIN_VIEW;
        }

        
        public void SetInGameGraphics(int chapter)
        {
            inGameBack.sprite = ResourceManager.GetInGameBackground(chapter);
            inGameFeatures.sprite = ResourceManager.GetInGameFeatures(chapter);
        }
        
        #endregion

        #region WordsView
        
        
        public void ClearWordsView()
        {
            for(int i = 0; i < wordsView.childCount; i++)
                Destroy(wordsView.GetChild(i).gameObject);    
        }

        public void AddToWordsView(string word)
        {
            GameObject g = Instantiate(acceptedWordPrefab, wordsView);
            g.GetComponentInChildren<Text>().text = word;
        }

        public void RemoveLastWord()
        {
            Destroy(wordsView.GetChild(wordsView.childCount - 1).gameObject);
        }
        
        #endregion

        
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

        public void Escape()
        {
            switch (state)
            {
                case GameViewState.MAIN_VIEW:
                    ShowPanel(pausePanel);
                    state = GameViewState.ONSCREEN;
                    break;
                
                case GameViewState.ONSCREEN:
                    GameObject panel = onScreenPanels.Pop();
                    panel.SetActive(false);

                    if (onScreenPanels.Count == 0)
                    {
                        state = lastState;
                    }
                    break;
            }
        }


        public void ShowWinPanel(int gem, int coinGain)
        {
            winPanel.SetActive(true);
            winHandler.Init(gem, coinGain);
        }
        
        
        #region Helpers
        
        private GameViewState lastState;
        private Stack<GameObject> onScreenPanels;
        private static readonly int TRIG_PANEL_OUT = Animator.StringToHash("Panel_Out");


        private void ShowPanel(GameObject panel)
        {
            onScreenPanels.Push(panel);
            panel.SetActive(true);
            lastState = state;
            state = GameViewState.ONSCREEN;
        }
        
        public void HidePanel(Animator panel)
        {
            panel.SetTrigger(TRIG_PANEL_OUT);
        }
        #endregion

    }
    
    
}

public enum GameViewState
{
    MAIN_VIEW,
    ONSCREEN
}
