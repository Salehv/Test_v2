using System.Collections;
using System.Collections.Generic;
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
        AudioManager.instance.PlaySFX(SFX.WHOOSH);
        AudioManager.instance.PlayNewMusic(AudioManager.instance.GetGameMusic(lvl.chapterId));
        textTop.text = lvl.begin;
        textBottom.text = lvl.end;
        selectedLevel = lvl;
        // TODO: Chapter name instead of number
        levelID.text = "فصل " + (lvl.chapterId + 1) + " - مرحله " + (lvl.id < 9 ? "0" : "") + (lvl.id + 1);
        
        animatorTop.SetBool("Close", true);
        animatorBottom.SetBool("Close", true);
        
        GetComponent<Image>().raycastTarget = true;
    }


    public void TransitionEnded()
    {
        if (selectedLevel == null)
            return;
        // AudioManager.instance.SetSFXPitch(1);
        AudioManager.instance.PlaySFX(SFX.COLLISION);
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(1);
        AudioManager.instance.PlaySFX(SFX.WHOOSH);
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