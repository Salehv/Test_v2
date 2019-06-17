using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaptersHandler : MonoBehaviour
{
    public GameObject chapterObjectPrefab;
    public Transform fphssContent;
    
    private List<ChapterObject> chapters;

    internal void InitializeChapters()
    {
        chapters = new List<ChapterObject>();
        
        for (int i = 0; i < GameManager.instance.chapters.Length; i++)
        {
            chapters.Add(fphssContent.GetChild(i).GetComponent<ChapterObject>());
            InitializeChapter(i, chapters[i]);
        }
    }

    private void InitializeChapter(int id, ChapterObject chapter)
    {
        var res = ResourceManager.instance;
        chapter.id = id;
        chapter.background.sprite = res.GetChapterSprite(id);
        
        chapter.CreateOnClick();
    }
    
    public void UpdateChaptersLockState()
    {
            
    }
    
    public void UpdateChapterGems(int id, int gems, int max)
    {
        chapters[id].chapterGems.text = Utilities.GetCompletionText(gems, max);
    }
    

    internal void UnlockAll()
    {
        for (int i = 1; i < GameManager.instance.chapters.Length; i++)
        {
            chapters[i].chapterLock.SetActive(false);
        }
    }


    
}