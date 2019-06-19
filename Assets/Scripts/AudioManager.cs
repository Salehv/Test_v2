using System.Collections;
using App;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    internal static AudioManager instance;

    [Header("Audio Clips")] 
    public AudioClip[] sfxClips;

    private bool musicOn = true, sfxOn = true;

    private void Awake()
    {
        instance = this;
    }


    public AudioClip GetGameMusic(int chapterId)
    {
        return ResourceManager.GetInGameMusic(chapterId);
    }

    public void PlaySFX(int sfx)
    {
        if (sfxOn)
        {
            PlayNewSfx(sfxClips[sfx]);
        }
    }

    public void PlaySFX(SFX sfx)
    {
        PlaySFX((int) sfx);
    }


    [Header("Audio Object")] public Transform audioObject;
    [Header("Prefabs")] public GameObject audioSourcePrefab;

    private AudioSource currentMusic;

    public void PlayNewMusic(AudioClip music)
    {
        if (currentMusic != null)
        {
            if (musicOn)
                StartCoroutine(FadeOut(currentMusic, 2));
            else
                Destroy(currentMusic.gameObject);
        }

        currentMusic = Instantiate(audioSourcePrefab, audioObject).GetComponent<AudioSource>();
        AudioSource source = currentMusic;
        source.loop = true;
        source.clip = music;
        source.volume = 0;
        source.Play();

        if (musicOn)
            StartCoroutine(FadeIn(source, 2));
    }

    public void PlayNewSfx(SFX sfx)
    {
        PlayNewSfx(sfxClips[(int) sfx]);
    }
    
    public void PlayNewSfx(AudioClip sfx)
    {
        if (!sfxOn)
            return;

        GameObject g = Instantiate(audioSourcePrefab, audioObject);
        AudioSource source = g.GetComponent<AudioSource>();
        source.clip = sfx;
        source.Play();

        StartCoroutine(RemoveSfx(source));
    }


    #region On-Off-Control

    public void ToggleMusic(Toggle t)
    {
        musicOn = !t.isOn;

        currentMusic.volume = !musicOn ? 0 : 1;

        ViewManager.instance.UpdateMusicState(musicOn, sfxOn);
    }

    public void ToggleSFX(Toggle t)
    {
        sfxOn = !t.isOn;

        ViewManager.instance.UpdateMusicState(musicOn, sfxOn);
    }

    #endregion

    #region Coroutines

    private IEnumerator RemoveSfx(AudioSource src)
    {
        while (src.isPlaying)
            yield return null;

        Destroy(src.gameObject);
    }

    private IEnumerator FadeIn(AudioSource src, float time)
    {
        float t = 0;
        while (t < time)
        {
            yield return null;
            t += Time.deltaTime;
            var v = (t / time);
            src.volume = v;
        }
    }

    private IEnumerator FadeOut(AudioSource src, float time)
    {
        float t = 0;
        while (t < time)
        {
            yield return null;
            t += Time.deltaTime;
            var v = 1 - t / time;
            src.volume = v;
        }

        Destroy(src.gameObject);
    }

    #endregion


    public AudioClip GetChapterMusic(int chapterId)
    {
        return ResourceManager.GetLevelsViewMusic(chapterId);
    }
}

public enum SFX
{
    WIN_1 = 9,
    WIN_2 = 10,
    WIN_3 = 11,
    GEM_TAKEN = 3,
    WIN = 4,
    CORRECT_WORD = 5,
    WRONG_WORD = 6,
    WHOOSH = 7,
    COLLISION = 8
}