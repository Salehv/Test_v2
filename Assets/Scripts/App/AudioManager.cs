using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    internal static AudioManager instance;

    private bool musicOn = true, sfxOn = true;

    private void Awake()
    {
        instance = this;
    }

    private Dictionary<SFX, AudioClip> sfxClips;

    internal void Init()
    {
        sfxClips = new Dictionary<SFX, AudioClip>();
        SFXClip[] clips = ResourceManager.GetSfxClips();

        foreach (SFXClip sfxClip in clips)
        {
            sfxClips[sfxClip.name] = sfxClip.clip;
        }
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
        PlayNewSfx(sfxClips[sfx]);
    }

    private void PlayNewSfx(AudioClip sfx)
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
}

[System.Serializable]
public struct SFXClip
{
    [SerializeField] public SFX name;
    [SerializeField] public AudioClip clip;
}

[System.Serializable]
public enum SFX
{
    UI_BUTTON_PRESSED,
    UI_PANEL_IN,
    UI_PANEL_OUT,
    UI_WHOOSH,
    UI_WHOOSH_REV,
    UI_UNLOCKED,
    UI_DENIED,
    GAME_WRONG_WORD,
    GAME_CORRECT_WORD,
    GAME_UNDO,
    GAME_WIN_1,
    GAME_WIN_2,
    GAME_WIN_3,
    GATES,
    GATES_COLLISION
}