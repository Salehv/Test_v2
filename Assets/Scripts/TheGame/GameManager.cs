using System;
using System.Collections.Generic;
using App;
using Initialize;
using TapsellSDK;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame
{
    public class GameManager : DynamicsHandler
    {
        internal static GameManager instance;
        public bool debugMode;

        [Header("Game Design")] 
        public int similarHintCost;
        public int showWayHintCost;
    
        internal Chapter[] chapters;
        private LinkedList<string> words;
        private string currentEndWord;
        private int currentShufflePage = 0;

        private Level currentLevel;

        [Header("Objects of game")] 
        public EditorHandler textEditor;
        public GameObject letterPrefab;
        public LetterPoolHandler letterPool;

        [Header("Game UI")] 
        private GameViewManager viewManager;
        public GameObject winPanel;
        public Image inGameBack;
        public Image inGameFeatures;
        public Text hintSimilarCostText;
        public Text hintWayCostText;

        public Button btnPrevShuffle;
        public Button btnNextShuffle;
        public Button btnHint;
        public Button btnHintShowWay;

        private GameState state;

        private DynamicsFlag LevelDynamicsFlag => currentLevel.flags;

        // TODO: move to ApplicationManager
        internal GameProgression progress;

        private int coins;
        private HintState hintState;


    
        #region Initialize


        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                DestroyImmediate(this);
        }


        private Sprite[] letterSprites;

        internal void Init(XMLGame game)
        {
            viewManager = GameViewManager.instance;
        
            coins = DatabaseManager.instance.GetCoins();


            chapters = new Chapter[game.chapters.Count];

            for (int i = 0; i < game.chapters.Count; i++)
            {
                chapters[i] = new Chapter();
                chapters[i].InitChapter(game.chapters[i]);
            }

            progress = GetProgress();

            letterSprites = Resources.LoadAll<Sprite>("Letters");

            hintToShow = new Queue<string>();

            hintSimilarCostText.text = similarHintCost + "";
            hintWayCostText.text = showWayHintCost + "";
        }

        #endregion

    
        internal void PlayLevel(Level lvl)
        {
            ResetGameStatus();
        
            currentLevel = lvl;
            currentEndWord = Utilities.GetNormalizedFarsi(lvl.end);
        
            state = GameState.MAIN_VIEW;

            viewManager.ClearWordsView();
            viewManager.SetInGameGraphics(lvl.chapterId);
            viewManager.SetEndWord(currentEndWord);
        
            textEditor.Initialize(this, lvl.begin, lvl.flags);
            words.AddFirst(Utilities.GetNormalizedFarsi(lvl.begin));
        
            ResetDynamics();
            int[] related = GetRelatedChars(Utilities.GetNormalizedFarsi(lvl.begin), currentShufflePage);
        
            letterPool.Init(textEditor, related);

            hintState = new HintState();


            /********* Tutorials *********/
            TimeManager.instance.SetTimer("CurrentGame");
            AnalyticsHandler.Level_Started(lvl.chapterId + 1, lvl.id + 1);


            // Undo
            if (lvl.id == 1 && lvl.chapterId == 0)
            {
                AddCorrectWord(Utilities.GetNormalizedFarsi("کن"));
                textEditor.Initialize(this, "کن", LevelDynamicsFlag);
                TutorialHandler.instance.Play_Tutorial_02();
                state = GameState.TUTORIAL;
                return;
            }
        

            // Hint
            if (lvl.id == 4 && lvl.chapterId == 0)
            {
                TutorialHandler.instance.Play_Tutorial_03();
                state = GameState.TUTORIAL;
            }

            // Add Letter
            if (lvl.id == 7 && lvl.chapterId == 0)
            {
                TutorialHandler.instance.Play_Tutorial_04();
                state = GameState.TUTORIAL;
                return;
            }

            // Remove Letter
            if (lvl.id == 11 && lvl.chapterId == 0)
            {
                TutorialHandler.instance.Play_Tutorial_05();
                state = GameState.TUTORIAL;
            }


            if (lvl.chapterId == 0 & lvl.id == 10 && !PlayerPrefs.HasKey("question_form_showed"))
            {
                ViewManager.instance.ShowQuestionForm();
                PlayerPrefs.SetInt("question_form_showed", 1);
                PlayerPrefs.Save();
            }
        }
    
        private void ResetGameStatus()
        {
            HideAllPanels();
            textEditor.ResetPluses();
        
            hintBlinkState = 0;
            hint_way_used = 0;
            hint_similar_used = 0;
        
            words = new LinkedList<string>();
        }
    
    
    
        #region UnclassifiedShit

        private int hint_way_used;
        private int hint_similar_used;

        private float gameTime;
        private int hintBlinkState;

    
        public int Tut01Completed()
        {
            var coinGain = 10;
            var solvedSteps = 3;

            // Calculate Coins
            if (progress.GetLevelProgress(currentLevel) == null)
                coinGain = currentLevel.CalculateCoinGain(solvedSteps - 1);
            AddCoins(coinGain);
            ApplicationManager.instance.UpdateCoins();


            // Calculate Gems
            var gem = 1;
            var minimumCount = currentLevel.way.Length;
            if (solvedSteps <= minimumCount)
                gem = 3;
            else if (solvedSteps < minimumCount * 2)
                gem = 2;

            UpdateCurrentLevelProgress(gem, solvedSteps);
            ApplicationManager.instance.UpdateGems();
            return coinGain;
        }


        private GameProgression GetProgress()
        {
            return DatabaseManager.instance.GetProgressData();
        }

        internal Chapter GetChapter(int n)
        {
            return chapters[n];
        }

        private void CheckWin()
        {
            if (GetLastWord() != currentEndWord)
                return;
            Win();
        }

        internal override string GetLastWord()
        {
            return words.Last.Value;
        }

        #endregion

        /***  Game Dynamics  ***/

        #region GameDynamics

        internal override bool ChangeLetter(int inTextPosition, int code)
        {
            if (debugMode)
                Debug.Log("Change Letter at [" + inTextPosition + "] to " + Utilities.dic_letterToChar[code]);

            string s = GetLastWord();

            if (inTextPosition == 0)
                s = Utilities.dic_letterToChar[code] + s.Substring(inTextPosition + 1);
            else if (inTextPosition == s.Length - 1)
                s = s.Substring(0, inTextPosition) + Utilities.dic_letterToChar[code];
            else
                s = s.Substring(0, inTextPosition) + Utilities.dic_letterToChar[code] + s.Substring(inTextPosition + 1);

            if (debugMode)
                Debug.Log("Word Request: " + s);

            if (DatabaseManager.instance.IsCorrect(s))
            {
                if (s == words.Last.Value)
                    return false;

                AddCorrectWord(s);
                textEditor.ChangeLetter(inTextPosition, code);

                return true;
            }

            return false;
        }

        internal override bool AddLetter(int inTextPosition, int code)
        {
            if (debugMode)
                Debug.Log("Add Letter [" + Utilities.dic_letterToChar[code] + "] between [" + inTextPosition + ", " +
                          (inTextPosition + 1) + "]");

            string s = GetLastWord();
            s = s.Substring(0, inTextPosition) + Utilities.dic_letterToChar[code] + s.Substring(inTextPosition);

            if (debugMode)
                Debug.Log("Word Request: " + s);

            if (DatabaseManager.instance.IsCorrect(s))
            {
                AddCorrectWord(s);
                textEditor.AddLetter(inTextPosition, code, currentLevel.flags.canRemoveLetter);
                return true;
            }

            return false;
        }

        internal override bool RemoveLetter(int inTextPosition)
        {
            if (debugMode)
                Debug.Log("Remove Letter at [" + inTextPosition + "]");

            var s = GetLastWord();
            print(s);
            if (inTextPosition == 0)
                s = s.Substring(1);
            else if (inTextPosition == s.Length - 1)
                s = s.Substring(0, inTextPosition);
            else
                s = s.Substring(0, inTextPosition) + s.Substring(inTextPosition + 1);

            if (debugMode)
                Debug.Log("Word Request: " + s);

            if (!DatabaseManager.instance.IsCorrect(s))
                return false;

            AddCorrectWord(s);
            return true;
        }

        public void Undo()
        {
            if (words.Count < 2)
                return;

            words.RemoveLast();
            viewManager.RemoveLastWord();

            textEditor.Initialize(this, words.Last.Value, currentLevel.flags);
            ResetDynamics();
            letterPool.Init(textEditor, GetRelatedChars(GetLastWord(), currentShufflePage));
        }

        private void AddCorrectWord(string s)
        {
            if(debugMode)
                Debug.Log("Word Added: " + s);
        
            words.AddLast(s);
        
            viewManager.AddToWordsView(words.Last.Previous.Value);
        
            letterPool.Init(textEditor, GetRelatedChars(s, currentShufflePage));
            textEditor.ResetPluses();
        
            CheckWin();
        }

        #endregion

        /*** UI Actions ***/

        #region UIAction

        private void HideAllPanels()
        {
            winPanel.SetActive(false);
            hintPanel.SetActive(false);
            hint_showWayPanel.SetActive(false);
            hint_similarWordsPanel.SetActive(false);
            shuffleLetterPanel.SetActive(false);
            shufflePanel.SetActive(false);
        }

        private void ShowWinPanel(int gemTaken, int coinTaken)
        {
            winPanel.SetActive(true);
            winPanel.GetComponent<WinPanelHandler>().Init(gemTaken, coinTaken);
        }


        [Header("Shuffle")] public GameObject shufflePanel;
        public GameObject shuffleLetterPanel;
        public GameObject poolBuyButton;

        private int maxShufflePage = 0;


        public void ShowShuffleMenu()
        {
            shufflePanel.SetActive(true);
            state = GameState.SHUFFLE_MENU;
        }

        public void ShowShuffleLetterSelectPanel()
        {
            shufflePanel.SetActive(false);
            shuffleLetterPanel.SetActive(true);
        }


        public void ShuffleLetterSelected(int letter)
        {
            Transform middle = letterPool.transform.GetChild(4);
            middle.GetComponent<AvailableLetter>().Init(textEditor, Utilities.dic_letterToChar[letter]);
            middle.GetComponent<AvailableLetter>().SetShine(true);

            Escape();
        }

        public void ShufflePageBought()
        {
            maxShufflePage += 1;
            if (maxShufflePage > 2)
            {
                maxShufflePage = 2;
                return;
            }

            else if (maxShufflePage == 2)
            {
                poolBuyButton.GetComponent<Button>().interactable = false;
            }

            // AddCoins(-20);
            btnNextShuffle.interactable = true;

            Escape();
        }

        public void NextShufflePage()
        {
            if (currentShufflePage == maxShufflePage)
                return;

            btnPrevShuffle.interactable = true;
            currentShufflePage += 1;
            if (currentShufflePage == maxShufflePage)
                btnNextShuffle.interactable = false;
            letterPool.Init(textEditor, GetRelatedChars(GetLastWord(), currentShufflePage));
        }

        public void PrevShufflePage()
        {
            if (currentShufflePage == 0)
                return;

            btnNextShuffle.interactable = true;
            currentShufflePage -= 1;
            if (currentShufflePage == 0)
                btnPrevShuffle.interactable = false;
            letterPool.Init(textEditor, GetRelatedChars(GetLastWord(), currentShufflePage));
        }

        private void ResetDynamics()
        {
            currentShufflePage = 0;
            btnPrevShuffle.interactable = false;
            btnNextShuffle.interactable = false;
            maxShufflePage = 0;
            poolBuyButton.GetComponent<Button>().interactable = true;

            EditorLetterHandler.RemoveDelete();
            textEditor.ResetPluses();
            AvailableLetter.Reset();


            //selectedLetter.GetComponent<SHAvailableLetter>().letterImage.color = Color.clear;
            //selectedLetter.GetComponent<SHAvailableLetter>().isOn = false;
        }

        #endregion

        #region Hint

        [Header("Hint")] public GameObject hintPanel;
        public GameObject hint_showWayPanel;
        public GameObject hint_similarWordsPanel;


        public void ShowHintMenu()
        {
            hintPanel.SetActive(true);
            state = GameState.HINT_PANEL;
        }


        private string[] shownSimilars;

        public void Hint_ShowSimilarWords()
        {
            try
            {
                AddCoins(-similarHintCost);
            }
            catch (NoMoneyException e)
            {
                PopupHandler.ShowNoMoney("شما " + (similarHintCost - coins) + " سکه نیاز دارید");
                print("poool nadari gooooooooooooozoooooooooooooo");
                return;
            }

            AnalyticsHandler.HintUsed(currentLevel.chapterId, currentLevel.id, HintType.SIMILAR);
            hint_similar_used += 1;
            hintPanel.SetActive(false);
            hint_similarWordsPanel.SetActive(true);
            hintState.canUseSimilar = false;

            var hsw = hint_similarWordsPanel.GetComponent<HintSimilarWordsHandler>();
            var sWords = DatabaseManager.instance.GetAllSimilarWords(GetLastWord());
            shownSimilars = new string[sWords.Length];

            for (int i = 0; i < hsw.swParent.childCount; i++)
                Destroy(hsw.swParent.GetChild(i).gameObject);

            int c = 0;
            foreach (var word in sWords)
            {
                if ((word.Length != words.Last.Value.Length) && (currentLevel.id < 7) && (currentLevel.chapterId == 0))
                    continue;

                else if ((word.Length < words.Last.Value.Length) && (currentLevel.id < 11) && (currentLevel.chapterId == 0))
                    continue;

                GameObject g = Instantiate(hsw.similarWordPrefab, hsw.swParent);
                g.name = c + "";
                g.GetComponentInChildren<Toggle>().group = hsw.tg;
                g.GetComponentInChildren<Text>().text = word;
                shownSimilars[c] = Utilities.GetNormalizedFarsi(word);
                c++;
            }
        }

        public ToggleGroup similarWordsTG;

        public void Hint_SimilarWord_Selected(ToggleGroup tg)
        {
            var ts = tg.GetComponentsInChildren<Toggle>();

            foreach (var t in ts)
            {
                if (t.isOn)
                {
                    int selected = int.Parse(t.transform.parent.name);
                    AddCorrectWord(shownSimilars[selected]);
                    textEditor.Initialize(this, shownSimilars[selected], currentLevel.flags);
                    break;
                }
            }
        }


        public GameObject hintWordPrefab;
        public GameObject hintWordShinePrefab;
        private Queue<string> hintToShow;

        public void Hint_ShowWay()
        {
            try
            {
                AddCoins(-showWayHintCost);
            }
            catch (NoMoneyException e)
            {
                PopupHandler.ShowNoMoney("شما " + (showWayHintCost - coins) + " سکه نیاز دارید");
                print("poool nadari gooooooooooooozoooooooooooooo");
                return;
            }

            AnalyticsHandler.HintUsed(currentLevel.chapterId, currentLevel.id, HintType.WAY);

            hintPanel.SetActive(false);
            hint_showWayPanel.SetActive(true);
        }

        internal void Hint_ShowWayStartAnimation()
        {
            hint_way_used += 1;
            var way = currentLevel.way;
            for (int i = 0; i < way.Length; i++)
            {
                hintToShow.Enqueue(way[i]);
            }

            NextWord();
        }

        public void NextWord()
        {
            if (hintToShow.Count > 0)
            {
                GameObject g = Instantiate(hintWordPrefab, hint_showWayPanel.transform);
                Instantiate(hintWordShinePrefab, hint_showWayPanel.transform);
                g.GetComponent<Text>().text = hintToShow.Dequeue();
                print(" shit");
            }
            else
            {
                HintWordAppeared.lastWord = true;
            }
        }

        public void EndShowWayHint()
        {
            hint_showWayPanel.SetActive(false);
        }

        #endregion

        #region GameActions

        public GameObject prizeButton;

        private void Win()
        {
            var coinGain = 0;
            var solvedSteps = words.Count;

            // Calculate Coins
            if (progress.GetLevelProgress(currentLevel) == null)
                coinGain = currentLevel.CalculateCoinGain(solvedSteps - 1);
            AddCoins(coinGain);

            _collectedCoinWaitingForReward = coinGain;

            if (coinGain == 0)
                prizeButton.SetActive(false);
            else
                prizeButton.SetActive(true);


            ApplicationManager.instance.UpdateCoins();


            // Calculate Gems
            var gem = currentLevel.CalculateGemGain(solvedSteps - 1);

            ResetDynamics();
            viewManager.ShowWinPanel(gem, coinGain);


            // Time
            int lvlTime = (int) TimeManager.instance.GetCurrentTime("CurrentGame");
            TimeManager.instance.DiscardTimer("CurrentGame");

            print("Level Time: " + lvlTime);

            int oldGem = 0;
            try
            {
                oldGem = progress.GetLevelProgress(currentLevel.chapterId, currentLevel.id).gemTaken;
            }
            catch (Exception e)
            {
            }

            AnalyticsHandler.Level_Completed(currentLevel.chapterId + 1, currentLevel.id + 1, gem - oldGem, coinGain,
                lvlTime);


            UpdateCurrentLevelProgress(gem, solvedSteps);

            ApplicationManager.instance.UpdateGems();

            if (chapters[currentLevel.chapterId].levels.Length <= currentLevel.id + 1)
                AnalyticsHandler.Chapter_Finished(currentLevel.chapterId, 0);
        }

        public void DoublePrize()
        {
            Tapsell.requestAd("5cd16a3ddae9a60001f48aec", false,
                (TapsellAd ad) => { Tapsell.showAd(ad, new TapsellShowOptions()); },
                (noAd) => { },
                (TapsellError e) => { },
                (string e) => { },
                (ad) => { }
            );
        }

        private int _collectedCoinWaitingForReward = 0;

        public void DoublePrizeReward()
        {
            AddCoins(_collectedCoinWaitingForReward);
            PopupHandler.ShowDebug("ایول! سکه شما دو برابر شد!");
            AnalyticsHandler.PrizeUsed(currentLevel.chapterId, currentLevel.id, _collectedCoinWaitingForReward);
            Exit();
        }

        public void Replay()
        {
            TimeManager.instance.DiscardTimer("CurrentGame");
            ResetDynamics();
            PlayLevel(currentLevel);
        }

        public void NextLevel()
        {
            if (chapters[currentLevel.chapterId].levels.Length > currentLevel.id + 1)
            {
                try
                {
                    EnergyHandler.instance.UseEnergy();

                    TransitionHandler.instance.StartTransition(chapters[currentLevel.chapterId]
                        .levels[currentLevel.id + 1]);
                }
                catch (NoEnergyException e)
                {
                    PopupHandler.ShowDebug("کلیدات تموم شده باید صبر کنی کلید پیدا کنم");
                }
            }
            else
            {
                Exit();
                if (PlayerPrefs.HasKey(string.Format("chapter_{0}_completed", currentLevel.chapterId)))
                    return;

                ChestHandler.instance.CallChest(100, 10, true, currentLevel.chapterId + 1);
                PlayerPrefs.SetInt(string.Format("chapter_{0}_completed", currentLevel.chapterId), 1);
                PlayerPrefs.Save();

            }
        }

        public void Exit()
        {
            if (chapters[currentLevel.chapterId].levels.Length > currentLevel.id + 1)
            {
                /*
            if (!PlayerPrefs.HasKey("chapter_" + currentLevel.chapterID + "_ad_played"))
            {
                PlayerPrefs.SetInt("chapter_" + currentLevel.chapterID + "_ad_played", 1);
                PlayerPrefs.Save();

                // TODO: Fix the issue
                // ApplicationManager.instance.ShowChapterEndAd();
            }
            */
            }

            ResetDynamics();
        
            ApplicationManager.instance.Game_ExitToMenu(currentLevel.chapterId);
        
            TimeManager.instance.DiscardTimer("CurrentGame");
        }

        public void Escape()
        {
            switch (state)
            {
                case GameState.HINT_PANEL:
                    hintPanel.SetActive(false);
                    hint_showWayPanel.SetActive(false);
                    hint_similarWordsPanel.SetActive(false);
                    state = GameState.MAIN_VIEW;
                    break;
                case GameState.SHUFFLE_MENU:
                    shufflePanel.SetActive(false);
                    shuffleLetterPanel.SetActive(false);
                    state = GameState.MAIN_VIEW;
                    break;
                case GameState.TUTORIAL:
                    TutorialHandler.instance.Escape();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateCurrentLevelProgress(int gem, int steps)
        {
            print("Gem: " + gem + ", Steps:" + steps);
            LevelProgression lp = new LevelProgression(currentLevel.chapterId, currentLevel.id, gem, steps);


            if (progress.GetLevelProgress(currentLevel) == null)
            {
                AddGems(gem);
                DatabaseManager.instance.UpdateLevelProgress(lp, false);
            }
            else if (gem > progress.GetLevelProgress(currentLevel).gemTaken)
            {
                AddGems(gem - progress.GetLevelProgress(currentLevel).gemTaken);
                DatabaseManager.instance.UpdateLevelProgress(lp, true);
            }

            progress.UpdateLevelProgress(lp);
        }

        public void AddCoins(int coin)
        {
            DatabaseManager.instance.AddOrRemoveCoins(coin);
            ApplicationManager.instance.UpdateCoins();
        }

        public void AddGems(int gem)
        {
            DatabaseManager.instance.AddOrRemoveGems(gem);

            ApplicationManager.instance.UpdateGems();
        }

        #endregion


        #region Helpers

        private int[] GetRelatedChars(string word, int page)
        {
            if (page > 2 || page < 0)
                return null;

            List<int> availableChars = new List<int>();

            string prior = currentEndWord;
            for (int i = 0; i < prior.Length; i++)
            for (int j = i + 1; j < prior.Length; j++)
                if (prior[i] == prior[j])
                    prior = prior.Remove(j);

            for (int i = 0; i < prior.Length; i++)
            {
                availableChars.Add(Utilities.dic_charToLetter[prior[i]]);
            }


            char[] possibleChars = DatabaseManager.instance.GetPossibleChars(word);
            if (possibleChars == null)
                possibleChars = new char[0];

            for (int i = 0; i < possibleChars.Length; i++)
            {
                if (!availableChars.Contains(Utilities.dic_charToLetter[possibleChars[i]]))
                {
                    availableChars.Add(Utilities.dic_charToLetter[possibleChars[i]]);
                }
            }

            for (int i = 0; i < 32; i++)
                if (!availableChars.Contains(i))
                    availableChars.Add(i);

            int[] result = new int[9];

            for (int i = page * 9; i < page * 9 + 9; i++)
                result[i - page * 9] = availableChars[i];

            return result;
        }

        public Sprite GetLetterSprite(int letter, SpriteMode mode)
        {
            return letterSprites[letter + (int) mode * 32];
        }

        #endregion

        /****** Getters And Setters *******/
        internal int GetCoins()
        {
            return coins;
        }


        public void TutorialEnded()
        {
            state = GameState.MAIN_VIEW;
        }


        private float hintTime = 0;

        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.J))
            {
                ShowWinPanel(1, 100);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                ShowWinPanel(2, 100);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                ShowWinPanel(3, 100);
            }

#endif

            if (ApplicationManager.instance.IsInGame() && state == GameState.MAIN_VIEW)
            {
                hintTime += Time.deltaTime;
            }


            if (hintTime > 30 && (hintBlinkState == 0) && state == GameState.MAIN_VIEW)
            {
                btnHint.GetComponent<Animator>().SetTrigger("Blink");
                hintBlinkState = 1;
                hintTime = 0;
            }

            if ((hintTime > 30) && (hintBlinkState == 1) && state == GameState.MAIN_VIEW)
            {
                btnHint.GetComponent<Animator>().SetTrigger("Blink");
                hintBlinkState = 1;
                hintTime = 0;
            }
        }

        internal void Solve()
        {
            if (!ApplicationManager.instance.IsInGame())
                return;
            ResetDynamics();
            TutorialHandler.instance.ResetAll();
            UpdateCurrentLevelProgress(3, 1);
            TimeManager.instance.DiscardTimer("CurrentGame");
            NextLevel();
        }
    }


    enum GameState
    {
        MAIN_VIEW,
        PAUSE,
        HINT_PANEL,
        SHUFFLE_MENU,
        TUTORIAL,
    }

    public enum SpriteMode
    {
        NORMAL = 0,
        GOLD = 1,
        CORRECT = 2,
        ERROR = 4,
        SHINE = 5
    }

    class HintState
    {
        public int hint_similar_used = 0;
        public int hint_shine_used = 0;
        public int hint_way_used = 0;

        public bool canUseSimilar = true;
        public bool canUseShine = true;
        public bool canUseWay = true;
    }

    public enum HintType
    {
        SIMILAR,
        WAY
    }
}