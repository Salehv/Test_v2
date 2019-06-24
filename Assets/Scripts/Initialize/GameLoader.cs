using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using App;
using GameAnalyticsSDK;
using Mono.Data.SqliteClient;
using TheGame;
using UnityEngine;

namespace Initialize
{
    public class GameLoader : MonoBehaviour
    {
        internal static GameLoader instance;

        public GameObject splashScreen;
        public TextAsset levelsXml;
        private XMLGame game;


        [Header("Update")] public string version;
        private string updateURL = "http://support.qolenj.ir/kalanjar/version.php?v=";
        public GameObject updatePanel;


        // loadingControl
        private bool loaded = false;
        private bool splashed = false;

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(this);
                return;
            }

            instance = this;

            TurnOffCanvases();
        }

        private void TurnOffCanvases()
        {
            ViewManager.instance.TurnOffCanvases();
        }

        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Input.multiTouchEnabled = false;
            GameAnalytics.Initialize();

            LoadGameLevels();

            StartCoroutine(Load());
        }

        private void LoadGameLevels()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLGame));
            game = (XMLGame) serializer.Deserialize(new MemoryStream(levelsXml.bytes));

            for (int c = 0; c < game.chapters.Length; c++)
            {
                for (int l = 0; l < game.chapters[c].levels.Length; l++)
                {
                    game.chapters[c].levels[l].start = Utilities.GetOnlyFarsi(game.chapters[c].levels[l].start);
                    game.chapters[c].levels[l].end = Utilities.GetOnlyFarsi(game.chapters[c].levels[l].end);

                    for (int w = 0; w < game.chapters[c].levels[l].way.words.Length; w++)
                    {
                        game.chapters[c].levels[l].way.words[w] =
                            Utilities.GetOnlyFarsi(game.chapters[c].levels[l].way.words[w]);
                    }
                }
            }
        }


        #region Load

        private IEnumerator Load()
        {
            // When in editor
            if (Application.platform != RuntimePlatform.Android)
            {
                /**********************        Words Database         *************************/
                WWW load = new WWW(Path.Combine(Application.streamingAssetsPath, "words.db3"));
                if (load.error != null)
                    Debug.Log(load.error);

                while (!load.isDone)
                    yield return null;

                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "words.db3"), load.bytes);
                Debug.Log("Copied Words_Database to " + Path.Combine(Application.persistentDataPath, "words.db3"));


                /*********************            Progress DataBase             *************************/
                if (!File.Exists(Path.Combine(Application.persistentDataPath, "progress.db3")))
                {
                    load = new WWW(Path.Combine(Application.streamingAssetsPath, "progress.db3"));

                    if (load.error != null)
                        Debug.Log(load.error);

                    while (!load.isDone)
                        yield return null;

                    File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "progress.db3"), load.bytes);
                    Debug.Log("Copied Progress_Database to " +
                              Path.Combine(Application.persistentDataPath, "progress.db3"));
                }
            }

            // In Android
            else
            {
                yield return CopyDataBases();
            }


            try
            {
                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString(updateURL + version);

                    Debug.Log(s);
                    if (s == "old")
                    {
                        ShowUpdateRequest();
                        yield break;
                    }
                }
            }
            catch (Exception e)
            {
                // ignored
            }

            loaded = true;
            LoadingFinished();
        }

        private IEnumerator CopyDataBases()
        {
            if (!PlayerPrefs.HasKey("db_version") || PlayerPrefs.GetString("db_version") != version)
            {
                using (WWW load = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "words.db3"))
                {
                    if (load.error != "")
                        Debug.LogError(load.error);

                    while (!load.isDone)
                        yield return null;

                    File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "words.db3"), load.bytes);
                    Debug.Log("Copied Words_Database to " + Path.Combine(Application.persistentDataPath, "words.db3"));
                }

                PlayerPrefs.SetString("db_version", version);
                PlayerPrefs.Save();
            }

            int lastCoin = 0;
            string path = "URI=file:" + Application.persistentDataPath + "/progress.db3";
            if (!PlayerPrefs.HasKey("new_progress_database"))
            {
                if (File.Exists(Path.Combine(Application.persistentDataPath, "progress.db3")))
                {
                    using (IDbConnection pConn = new SqliteConnection(path))
                    {
                        pConn.Open();
                        using (IDbCommand cmd = pConn.CreateCommand())
                        {
                            string sqlQuery = "SELECT * FROM `datas` WHERE `option`='coin';";
                            cmd.CommandText = sqlQuery;
                            using (IDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                    lastCoin = reader.GetInt32(1);
                            }
                        }
                    }

                    File.Delete(Path.Combine(Application.persistentDataPath, "progress.db3"));
                }

                PlayerPrefs.SetInt("new_progress_database", 1);
                PlayerPrefs.Save();
            }

            if (!File.Exists(Path.Combine(Application.persistentDataPath, "progress.db3")))
            {
                using (WWW load2 = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "progress.db3"))
                {
                    if (load2.error != null)
                        Debug.Log(load2.error);

                    while (!load2.isDone)
                        yield return null;

                    File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "progress.db3"), load2.bytes);
                    Debug.Log(
                        "Copied Progress_Database to " + Path.Combine(Application.persistentDataPath, "progress.db3"));

                    using (IDbConnection pConn = new SqliteConnection(path))
                    {
                        pConn.Open();
                        using (IDbCommand cmd = pConn.CreateCommand())
                        {
                            string sqlQuery = $"UPDATE `datas` SET `value` ={lastCoin} WHERE `option`='coin';";
                            cmd.CommandText = sqlQuery;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void LoadingFinished()
        {
            if (loaded && splashed)
            {
                splashScreen.SetActive(false);
                DatabaseManager.instance.Init();
                GameManager.instance.Init(game);
                ApplicationManager.instance.Init();
            }
        }

        #endregion


        private void ShowUpdateRequest()
        {
            updatePanel.SetActive(true);
        }

        public void UpdateGame()
        {
            Application.OpenURL("market://details?id=ir.qolenj.kalanjar");
        }


        public void SplashEnded()
        {
            splashed = true;
            LoadingFinished();
        }
    }
}