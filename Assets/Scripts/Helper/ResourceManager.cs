using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;

    [Header("Sprites")] [SerializeField] private Sprite[] levelMenuSprites;

    [SerializeField] private Sprite[] chapterBluredBackgrounds;
    [SerializeField] private Sprite[] chapterBackgrounds;

    [SerializeField] private Sprite[] inGameBacks;
    [SerializeField] private Sprite[] inGameFeatures;

    [Header("Music & Sounds")] [SerializeField]
    private AudioClip[] levelsViewMusics;

    [SerializeField] private AudioClip[] inGameMusic;
    [SerializeField] private SFXClip[] sfxClips;
    [Space(15)] [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip chaptersMusic;


    private void Awake()
    {
        instance = this;
    }

    internal static Sprite GetLevelSprite(int chapter)
    {
        return instance.levelMenuSprites[chapter];
    }

    internal static Sprite GetChapterBlurredBackground(int chapter)
    {
        return instance.chapterBluredBackgrounds[chapter];
    }

    internal static Sprite GetInGameBackground(int chapter)
    {
        return instance.inGameBacks[chapter];
    }

    internal static Sprite GetInGameFeatures(int chapter)
    {
        return instance.inGameFeatures[chapter];
    }

    internal static Sprite GetChapterSprite(int id)
    {
        return instance.chapterBackgrounds[id];
    }

    public static AudioClip GetLevelsViewMusic(int chapterId)
    {
        return instance.levelsViewMusics[chapterId];
    }

    public static AudioClip GetInGameMusic(int chapterId)
    {
        return instance.inGameMusic[chapterId];
    }

    public static AudioClip GetMainMenuMusic()
    {
        return instance.mainMenuMusic;
    }

    internal static SFXClip[] GetSfxClips()
    {
        return instance.sfxClips;
    }

    public static AudioClip GetChaptersMusic()
    {
        return instance.chaptersMusic;
    }
}