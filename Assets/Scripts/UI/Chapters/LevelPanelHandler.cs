using System;
using System.Collections.Generic;
using TheGame;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanelHandler : MonoBehaviour
{
    public Transform contents;

    private List<LevelObject> levels;
    internal void Init()
    {
        levels = new List<LevelObject>();

        for (int i = 0; i < contents.childCount; i++)
        {
            levels.Add(contents.GetChild(i).GetComponent<LevelObject>());
        }
    }
    
    internal void SetToChapter(int chapter)
    {
        bool lastLevel = false;
        for (int i = 0; i < GameManager.instance.GetChapter(chapter).levels.Length; i++)
        {
            levels[i].gameObject.SetActive(true);
            bool isSolved = ApplicationManager.instance.GetLevelProgress(chapter, i) != null;

            SetLevel(levels[i], chapter, i, isSolved, lastLevel);
            
            if (!isSolved && !lastLevel)
                lastLevel = true;
        }

        for (int i = GameManager.instance.GetChapter(chapter).levels.Length; i < 32; i++)
        {
            levels[i].gameObject.SetActive(false);
        }
    }

    
    private void SetLevel(LevelObject levelObject, int chapterId, int levelId, bool isSolved = false, bool isLocked = false)
    {
        levelObject.level = GameManager.instance.GetChapter(chapterId).GetLevel(levelId);
        try
        {
            levelObject.level.gems =
                ApplicationManager.instance.GetLevelProgress(chapterId, levelId).gemTaken;
        }
        catch (NullReferenceException e)
        {
            levelObject.level.gems = 0;
        }
        
        levelObject.Init();
        
        if (isLocked)
        {
            levelObject.SetLock();
            return;
        }

        if (isSolved)
        {
            levelObject.SetSolved();
            return;
        }
        
        levelObject.SetUnSolved(PlayerPrefs.GetInt("last_level_unlocked") == 1);
    }
}