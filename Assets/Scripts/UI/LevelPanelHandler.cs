using UnityEngine;
using UnityEngine.UI;


public class LevelPanelHandler : MonoBehaviour
{
    public GameObject passedLevelPrefab;
    public GameObject levelPrefab;
    public GameObject lockedLevelPrefab;

    internal void Init(int chapter)
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        for (int i = 0; i < GameManager.instance.GetChapter(chapter).levels.Length; i++)
        {
            if (GameManager.instance.progress.GetLevelProgress(chapter, i) != null)
            {
                CreateSolvedLevel(chapter, i);
            }
            else if (i == 0 || GameManager.instance.progress.GetLevelProgress(chapter, i - 1) != null)
            {
                CreateLevel(chapter, i);
            }
            else
            {
                CreateLockedLevel(chapter);
            }
        }
    }

    private void CreateSolvedLevel(int chapterId, int levelId)
    {
        GameObject g = Instantiate(passedLevelPrefab, transform, true);

        g.GetComponent<LevelObject>().level = GameManager.instance.GetChapter(chapterId).GetLevel(levelId);
        g.GetComponent<LevelObject>().level.gems =
            GameManager.instance.progress.GetLevelProgress(chapterId, levelId).gemTaken;

//        print("Chap: " + chapterId + ", Lvl:" + levelId + ", gem:" +
//              GameManager.instance.progress.GetLevelProgress(chapterId, levelId).gemTaken);
        g.GetComponent<LevelObject>().Init();

        g.transform.localScale = Vector3.one;
    }

    private void CreateLevel(int chapter, int levelId)
    {
        GameObject g = Instantiate(levelPrefab, transform, true);
        g.transform.localPosition = Vector3.zero;
        g.GetComponent<LevelObject>().level = GameManager.instance.GetChapter(chapter).GetLevel(levelId);
        g.GetComponent<LevelObject>().Init();
        g.transform.localScale = Vector3.one;
    }

    private void CreateLockedLevel(int chapterID)
    {
        GameObject g = Instantiate(lockedLevelPrefab, transform, true);
        g.GetComponent<Image>().sprite = ResourceManager.instance.GetChapterLevelMenuSprite(chapterID);
        g.transform.localPosition = Vector3.zero;
        g.transform.localScale = Vector3.one;
    }
}