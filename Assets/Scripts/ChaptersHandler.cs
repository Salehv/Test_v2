using System.Collections;
using System.Collections.Generic;
using App;
using TheGame;
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
        chapter.id = id;
        chapter.background.sprite = ResourceManager.GetChapterSprite(id);
        chapter.chapterLockBackground.sprite = ResourceManager.GetChapterBlurredBackground(id);
        chapter.chapterName.text = GameManager.instance.chapters[id].name;

        chapter.CreateOnClick();
    }

    public void UpdateChaptersLockState()
    {
        int totalGems = ApplicationManager.instance.GetAllGemCount();

        for (int i = 0; i < chapters.Count; i++)
        {
            chapters[i].SetLockState(GameManager.instance.chapters[i].cost, totalGems);
        }
    }

    public void UpdateChapterGems(int id, int gems, int max)
    {
        chapters[id].chapterGems.text = gems + "";
        chapters[id].chapterAllGems.text = max + "";
    }

    internal void UnlockAll()
    {
        for (int i = 1; i < GameManager.instance.chapters.Length; i++)
        {
            chapters[i].chapterLock.SetActive(false);
        }
    }
}