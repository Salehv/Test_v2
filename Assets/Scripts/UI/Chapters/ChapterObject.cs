using App;
using UnityEngine;
using UnityEngine.UI;

public class ChapterObject : MonoBehaviour
{
    public int id;
    public bool isLocked;
    
    public Image background;

    [Space(10)] public Text chapterName;
    public Text chapterGems;
    public Text chapterAllGems;

    [Space(10)] public GameObject chapterLock;
    public Image chapterLockBackground;
    public Text chapterLockGem;


    [Space(10)] 
    public Button playButton;
    public Button chapterNameButton;

    internal void CreateOnClick()
    {
        playButton.onClick.AddListener(LevelRequest);
        chapterNameButton.onClick.AddListener(LevelRequest);
    }

    private void LevelRequest()
    {
        ViewManager.instance.ChaptersToLevels(id);
    }

    public void SetLockState(int cost, int totalGems)
    {
        if (cost > totalGems)
        {
            chapterLock.SetActive(true);
            chapterLockGem.text = Utilities.GetCompletionText(totalGems, cost);
            return;
        }
        
        chapterLock.SetActive(false);
    }
}
