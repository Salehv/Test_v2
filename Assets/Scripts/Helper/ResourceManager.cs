using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    internal static ResourceManager instance;

    [Header("Sprites")] 
    [SerializeField] private Sprite[] levelMenuSprites;

    [SerializeField] private Sprite[] chapterBluredBackgrounds;
    [SerializeField] private Sprite[] chapterBackgrounds;

    [SerializeField] private Sprite[] inGameBacks;
    [SerializeField] private Sprite[] inGameFeatures;

    [Header("Music & Sounds")] 
    [SerializeField] private AudioClip[] levelsViewMusics;
    [SerializeField] private AudioClip[] inGameMusic;
    [SerializeField] private AudioClip mainMenuMusic;
    
    
    private void Awake()
    {
        instance = this;
    }

    internal Sprite GetLevelSprite(int chapter)
    {
        return levelMenuSprites[chapter];
    }

    internal Sprite GetChapterBluredBackground(int chapter)
    {
        return chapterBluredBackgrounds[chapter];
    }

    internal Sprite GetInGameBackground(int chapter)
    {
        return inGameBacks[chapter];
    }

    internal Sprite GetInGameFeatures(int chapter)
    {
        return inGameFeatures[chapter];
    }

    internal Sprite GetChapterSprite(int id)
    {
        return chapterBackgrounds[id];
    }

    public AudioClip GetLevelsViewMusic(int chapterId)
    {
        return levelsViewMusics[chapterId];
    }

    public AudioClip GetInGameMusic(int chapterId)
    {
        return inGameMusic[chapterId];
    }

    public AudioClip GetMainMenuMusic()
    {
        return mainMenuMusic;
    }
}