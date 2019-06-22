using System.Collections.Generic;
using System.Runtime.InteropServices;
using App;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.Arcade
{
    public class ArcadeManager : DynamicsHandler
    {
        internal static ArcadeManager instance;
        public GameObject wordPrefab;
        public GameObject wordsView;
        public Panel endPanel;
        public Panel pausePanel;
        public Panel introPanel;
        public Text endScore;
        public Text highScore;

        public GameObject highScoreBeaten;

        public Text timeText;
        private float remainingTime;


        public bool debugMode;

        [Header("Game Objects")] public EditorHandler editor;

        public LetterPoolHandler letterPool;


        private List<string> words;
        private bool started = false;

        private void Awake()
        {
            instance = this;
        }

        public void StartArcade()
        {
            ViewManager.instance.SetEscapable();
            ViewManager.instance.Escape();
            started = true;
        }

        private void Update()
        {
            if (started)
            {
                timeText.text = Utilities.GetTimeFormat(remainingTime);
                remainingTime -= Time.deltaTime;
                if (remainingTime < 10)
                {
                    timeText.color = Color.red;
                }
                else
                {
                    timeText.color = Color.white;
                }

                if (remainingTime < 0)
                {
                    End();
                }
            }
        }


        internal void PlayArcade(string start, float time)
        {
            highScoreBeaten.SetActive(false);
            endPanel.gameObject.SetActive(false);

            words = new List<string>();

            for (int i = 0; i < wordsView.transform.childCount; i++)
            {
                Destroy(wordsView.transform.GetChild(i).gameObject);
            }


            letterPool.Init(editor, GetRandomCharacters());

            words.Add(start);

            editor.Initialize(this, start, DynamicsFlag.DF_FULL);

            remainingTime = time;
            timeText.text = Utilities.GetTimeFormat(remainingTime);
            timeText.color = Color.white;

            ViewManager.instance.SetUnEscapable();
            ViewManager.instance.ShowPanel(introPanel);
        }


        internal override string GetLastWord()
        {
            return words[words.Count - 1];
        }

        private void AddWordToView(string s)
        {
            GameObject w = Instantiate(wordPrefab, wordsView.transform, true);
            w.GetComponent<Text>().text = s;
            w.transform.localScale = Vector3.one;
        }

        private void AddCorrectWord(string word)
        {
            AddWordToView(GetLastWord());
            words.Add(word);
            letterPool.Init(editor, GetRandomCharacters());
            editor.ResetPluses();
        }


        private void End()
        {
            int score = words.Count - 1;
            started = false;

            ViewManager.instance.SetUnEscapable();
            ViewManager.instance.ShowPanel(endPanel);

            endScore.text = score + "";
            int hs = PlayerPrefs.GetInt("arcade_high_score");

            if (hs < score)
            {
                PlayerPrefs.SetInt("arcade_high_score", score);
                hs = score;
                highScoreBeaten.SetActive(true);
            }

            highScore.text = hs + "";
        }

        public void Stop()
        {
            started = false;
            ViewManager.instance.EndArcade();
        }


        #region Dynamics

        internal override bool ChangeLetter(int inTextPosition, int code)
        {
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
                if (s == GetLastWord())
                    return false;

                AddCorrectWord(s);

                editor.ChangeLetter(inTextPosition, code);

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

                editor.AddLetter(inTextPosition, code, true);
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

        #endregion


        #region Helpers

        private System.Random rand = new System.Random();

        private int[] GetRandomCharacters()
        {
            List<int> availableChars = new List<int>();

            while (availableChars.Count < 9)
            {
                int i = rand.Next(0, 32);

                if (!availableChars.Contains(i))
                    availableChars.Add(i);
            }

            return availableChars.ToArray();
        }

        #endregion
    }
}