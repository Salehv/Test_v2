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

        public void ShowGame()
        {
            gamePanelsObject.SetActive(true);
            state = GameViewState.MAIN_VIEW;
        }

        public void ClearWordsView()
        {
            for(int i = 0; i < wordsView.childCount; i++)
                Destroy(wordsView.GetChild(i).gameObject);    
        }

        public void SetInGameGraphics(int chapter)
        {
            inGameBack.sprite = ResourceManager.instance.GetInGameBackground(chapter);
            inGameFeatures.sprite = ResourceManager.instance.GetInGameFeatures(chapter);
        }

        public void AddToWordsView(string word)
        {
            GameObject g = Instantiate(acceptedWordPrefab, wordsView);
            g.GetComponentInChildren<Text>().text = word;
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

        public void Escape()
        {
            switch (state)
            {
                case GameViewState.MAIN_VIEW:
                    ViewManager.instance.ShowPanel(pausePanel);
                break;
            }
        }


        public void ShowWinPanel(int gem, int coinGain)
        {
        
        }
    }
}

public enum GameViewState
{
    MAIN_VIEW,
}
