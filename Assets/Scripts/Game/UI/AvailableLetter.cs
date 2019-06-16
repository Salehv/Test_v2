﻿using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvailableLetter : MonoBehaviour, IPointerDownHandler //, IPointerUpHandler //, IDragHandler
{
    internal static AvailableLetter currentLetter = null;
    private EditorHandler _editor;

    public Image letterImage;
    public int code;
    public GameObject shine;


    private int index;


    public void Init(EditorHandler editor, char c)
    {
        _editor = editor;

        code = GameManager.dic_charToLetter[c];
        letterImage.sprite = GameManager.instance.GetLetterSprite(code, SpriteMode.NORMAL);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentLetter != null)
            currentLetter.SetShine(false);

        if (currentLetter == this)
        {
            currentLetter = null;
            _editor.HidePluses();
            return;
        }

        if (currentLetter == null)
        {
            _editor.ShowPluses();
            EditorLetterHandler.RemoveDelete();
        }

        SetShine(true);
        currentLetter = this;
    }


    public void SetShine(bool isShinning)
    {
        shine.SetActive(isShinning);
        if (isShinning)
        {
            GetComponentInChildren<Animator>().SetTrigger("select");
            letterImage.sprite = GameManager.instance.GetLetterSprite(code, SpriteMode.SHINE);
        }
        else
        {
            letterImage.sprite = GameManager.instance.GetLetterSprite(code, SpriteMode.NORMAL);
        }
    }

    internal static void Reset()
    {
        if (currentLetter == null)
            return;

        currentLetter.SetShine(false);
        currentLetter = null;
    }
}