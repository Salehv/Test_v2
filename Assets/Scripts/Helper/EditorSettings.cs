#if UNITY_EDITOR

using System;
using App;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using TheGame;
using UnityEditor.PackageManager;

public class EditorSettings
{
    [MenuItem("Kalanjar/TEXT - Create chapters file")]
    private static void NewMenuOption()
    {
        Chapter[] cc = GameObject.Find("Manager").GetComponent<GameManager>().chapters;
        string s = "";
        foreach (Chapter c in cc)
        {
            foreach (Level l in c.levels)
            {
                foreach (string ss in l.way)
                {
                    s += ss + "\n";
                }
            }
        }

        System.IO.File.WriteAllText(@"all_levels.txt", Utilities.GetNormalizedFarsi(s));
        Debug.Log("Done");
    }

    [MenuItem("Kalanjar/JSON - Create chapters file")]
    private static void GetJson()
    {
        Chapter[] cc = GameObject.Find("Manager").GetComponent<GameManager>().chapters;
        LVLObject[] lvls = new LVLObject[100];
        int i = 0;
        foreach (Chapter c in cc)
        {
            foreach (Level l in c.levels)
            {
                lvls[i] = new LVLObject();
                lvls[i].id = c.id * 100 + l.id;
                lvls[i].From = l.begin;
                lvls[i].To = l.end;
                i++;
            }
        }

        string s = JsonConvert.SerializeObject(lvls);

        System.IO.File.WriteAllText(@"all_levels.json", s); //GameManager.NormalizeText(s));
        Debug.Log("Done");
    }

    [MenuItem("Kalanjar/CSV - Create chapters file")]
    private static void GetCSV()
    {
        Chapter[] cc = GameObject.Find("Manager").GetComponent<GameManager>().chapters;
        string s = "";
        foreach (Chapter c in cc)
        {
            foreach (Level l in c.levels)
            {
                s += l.begin + ",";
                s += l.end + "\n";
            }
        }

        System.IO.File.WriteAllText(@"all_levels.csv", s); //GameManager.NormalizeText(s));
        Debug.Log("Done");
    }

    [MenuItem("Kalanjar/Set Ad remaining time")]
    private static void SetTime()
    {
        PlayerPrefs.SetString("adWatched", DateTime.UtcNow.ToFileTimeUtc() / 10000000 + "");
        PlayerPrefs.Save();
    }

    [MenuItem("Kalanjar/Add 100 Coins")]
    private static void Add100Coins()
    {
        GameManager.instance.AddCoins(100);
    }

    [MenuItem("Kalanjar/First Play")]
    private static void FirstPlay()
    {
        PlayerPrefs.DeleteKey("first_enter_done");
        PlayerPrefs.DeleteKey("question_form_showed");
    }

    [MenuItem("Kalanjar/Remove Prefs")]
    private static void RemovePrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Kalanjar/Reset All Progress")]
    private static void RemoveAll()
    {
        ApplicationManager.instance.DEV_FirstPlay();
    }

    [MenuItem("Kalanjar/Unlock All Chapters")]
    private static void unlockAll()
    {
        ApplicationManager.instance.DEV_UnlockAllChapters();
    }

    [MenuItem("Kalanjar/Reset Key Charger")]
    private static void reset()
    {
        PlayerPrefs.SetString("lastChargeTime", "-1");
        PlayerPrefs.Save();
    }

    [MenuItem("Kalanjar/Add Key")]
    private static void add()
    {
        KeyHandler.instance.AddKeys(1);
    }
}

[Serializable]
class LVLObject
{
    [SerializeField] public int id;
    [SerializeField] public string From;
    [SerializeField] public string To;
}
#endif