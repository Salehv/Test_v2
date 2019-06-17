using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    internal static ResourceManager instance;

    [Header("Level Object Sprites")] 
    public Sprite[] levelMenuSprites;

    [Header("Chapter Images")] 
    public Sprite[] chapterBluredBackgrounds;
    public Sprite[] chapterBackgrounds;

    [Header("Chapter in Game Graphics")] public Sprite[] inGameBacks;
    public Sprite[] inGameFeatures;


    private void Awake()
    {
        instance = this;
    }

    internal Sprite GetChapterLevelMenuSprite(int chapter)
    {
        return levelMenuSprites[chapter];
    }

    internal Sprite GetChapterBluredBackground(int chapter)
    {
        return chapterBluredBackgrounds[chapter];
    }

    internal Sprite GetChapterInGameBackground(int chapter)
    {
        return inGameBacks[chapter];
    }

    internal Sprite GetChapterInGameFeaturesSprite(int chapter)
    {
        return inGameFeatures[chapter];
    }

    internal Sprite GetChapterSprite(int id)
    {
        return chapterBackgrounds[id];
    }
}