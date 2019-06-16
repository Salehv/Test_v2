using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UPersian.Components;

public class PopupHandler : MonoBehaviour
{
    internal static PopupHandler instance;

    internal static void ShowDebug(string text)
    {
        instance.CreateNewDebug(text);
    }

    internal static void ShowInfo(string text, UnityAction onOk)
    {
        instance.CreateNewInfo(text, onOk);
    }

    internal static void ShowNoMoney(string title)
    {
        instance.CreateNoMoneyPopup(title);
    }

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
    }


    public Transform popupParent;
    [Space(10)] public GameObject popupDebugObj;
    public GameObject popupInfoObj;
    public GameObject popupChoiceObj;


    private void CreateNewDebug(string text)
    {
        GameObject g = Instantiate(popupDebugObj, popupParent);
        g.GetComponentInChildren<RtlText>().text = text;
    }

    private void CreateNewInfo(string text, UnityAction action = null, string actionText = "باشه")
    {
        GameObject g = Instantiate(popupInfoObj, popupParent);
        g.GetComponent<PopupInfo>().mainText.text = text;

        Button b = g.GetComponent<PopupInfo>().actionButton;
        b.onClick.AddListener(action);
    }

    private void CreateNoMoneyPopup(string title)
    {
        GameObject g = Instantiate(popupChoiceObj, popupParent);
        g.SetActive(true);
        g.GetComponent<PopupNoMoney>().mainText.text = title;
    }
}