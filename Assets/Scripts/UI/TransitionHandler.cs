using System.Collections;
using System.Collections.Generic;
using App;
using TheGame;
using UnityEngine;
using UnityEngine.UI;

public class TransitionHandler : MonoBehaviour
{
    public static TransitionHandler instance;
    public Animator animatorTop;
    public Animator animatorBottom;
    public Text textTop;
    public Text textBottom;
    public Text levelID;


    private void Awake()
    {
        instance = this;
    }


    private Level selectedLevel = null;

    public void StartTransition(Level lvl)
    {
        Invoke($"PlayTransition", 0.7f);
        AudioManager.instance.PlayNewSfx(SFX.GATES);
        AudioManager.instance.PlayNewMusic(ResourceManager.GetInGameMusic(lvl.chapterId));
        textTop.text = lvl.begin;
        textBottom.text = lvl.end;
        selectedLevel = lvl;

        levelID.text = $"{GameManager.instance.chapters[lvl.chapterId].name} - مرحله {lvl.id + 1:00}";

        GetComponent<Image>().raycastTarget = true;
    }

    private void PlayTransition()
    {
        animatorTop.SetBool("Close", true);
        animatorBottom.SetBool("Close", true);
    }


    public void TransitionEnded()
    {
        if (selectedLevel == null)
            return;
        // AudioManager.instance.SetSFXPitch(1);
        // AudioManager.instance.PlayNewSfx(SFX.GATES_COLLISION);
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(0.5f);
        ApplicationManager.instance.RunLevel(selectedLevel);
        animatorTop.SetBool("Close", false);
        animatorBottom.SetBool("Close", false);
        selectedLevel = null;
        GetComponent<Image>().raycastTarget = false;
    }

//    
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            StartTransition("خر", "سگ");
//        }
//    }
}