using System;
using System.Collections;
using System.Collections.Generic;
using App;
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
        ViewManager.instance.SetUnEscapable();
    }

    internal static void ShowInfo(string text, UnityAction onOk)
    {
        instance.CreateNewInfo(text, onOk);
    }

    internal static void ShowNoMoney(int need)
    {
        instance.ShowNoMoney($"شما به {need} عدد سکه نیاز دارید");
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
    public Panel popupNoMoney;


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

    private void ShowNoMoney(string title)
    {
        popupNoMoney.GetComponent<PopupNoMoney>().mainText.text = title;
        ViewManager.instance.ShowPanel(popupNoMoney);
    }


    #region TutorialAutomation

    [Header("Tutorial Automation")] public GameObject tutMessagePrefab;
    public GameObject arrowTut;
    public GameObject defaultMessageParent;
    public Vector2 messageLargeSize;
    public Vector2 messageMediumSize;
    public Vector2 messageSmallSize;

    internal GameObject currentMessage;
    private GameObject currentArrow;


    public static void CreateMessage(string msg, Size size, Direction direction, GameObject destinationPanel = null,
        bool haveNextBtn = false, bool haveIcon = true)
    {
        if (destinationPanel == null)
            destinationPanel = instance.defaultMessageParent;

        instance.currentMessage = Instantiate(instance.tutMessagePrefab, destinationPanel.transform, true);
        instance.currentMessage.GetComponentInChildren<RtlText>().text = msg;

        if (size == Size.Large)
        {
            instance.currentMessage.GetComponent<RectTransform>().sizeDelta = instance.messageLargeSize;
            instance.currentMessage.transform.GetChild(2).transform.position = new Vector3(
                instance.currentMessage.transform.GetChild(2).transform.position.x,
                -instance.messageLargeSize.y / 2 - 20);
        }

        if (size == Size.Medium)
        {
            instance.currentMessage.GetComponent<RectTransform>().sizeDelta = instance.messageMediumSize;
            instance.currentMessage.transform.GetChild(2).transform.position = new Vector3(
                instance.currentMessage.transform.GetChild(2).transform.position.x,
                -instance.messageMediumSize.y / 2 - 20);
        }

        if (size == Size.Small)
        {
            instance.currentMessage.GetComponent<RectTransform>().sizeDelta = instance.messageSmallSize;
            instance.currentMessage.transform.GetChild(2).transform.position = new Vector3(
                instance.currentMessage.transform.GetChild(2).transform.position.x,
                -instance.messageSmallSize.y / 2 - 20);
        }


        float messageX;

        if (haveIcon == false)
        {
            instance.currentMessage.transform.GetChild(1).gameObject.SetActive(false);
            messageX = 0.45f;
        }
        else
        {
            instance.currentMessage.transform.GetChild(1).gameObject.SetActive(true);
            messageX = 0.5f;
        }

        if (haveNextBtn == false)
        {
            instance.currentMessage.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            instance.currentMessage.transform.GetChild(2).gameObject.SetActive(true);
        }

        instance.currentMessage.GetComponent<RectTransform>().localScale = Vector3.one;

        if (direction == Direction.Top)
        {
            RectTransform rt = instance.currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.7f);
            rt.anchorMin = new Vector2(messageX, 0.7f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Topmost)
        {
            RectTransform rt = instance.currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.8f);
            rt.anchorMin = new Vector2(messageX, 0.8f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Down)
        {
            RectTransform rt = instance.currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.3f);
            rt.anchorMin = new Vector2(messageX, 0.3f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Downmost)
        {
            RectTransform rt = instance.currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.13f);
            rt.anchorMin = new Vector2(messageX, 0.13f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Middle)
        {
            RectTransform rt = instance.currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.5f);
            rt.anchorMin = new Vector2(messageX, 0.5f);
            rt.anchoredPosition = new Vector2(38, 0);
        }
    }

    public static void DeleteMessage()
    {
        foreach (var animator in instance.currentMessage.GetComponentsInChildren<Animator>())
        {
            animator.SetTrigger("Hide");
        }
    }

    public static void DeleteMessageImmediate()
    {
        Destroy(instance.currentMessage);
    }


    public void CreateArrow(Transform objTransform, Direction direction, GameObject destinationPanel,
        Vector2 offset = default(Vector2))
    {
        currentArrow = Instantiate(arrowTut, destinationPanel.transform);
        currentArrow.transform.position = objTransform.position;
        (currentArrow.transform as RectTransform).anchoredPosition += offset;

        if (direction == Direction.Top)
        {
            currentArrow.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        if (direction == Direction.Left)
        {
            currentArrow.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        if (direction == Direction.Down)
        {
            currentArrow.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }

    public static void DeleteArrow()
    {
        Destroy(instance.currentArrow.gameObject);
    }

    public void ShineObjects(GameObject[] go)
    {
    }

    #endregion

    #region Pointer

    [Header("Pointer")] public GameObject pointer;

    public static void ShowPointerClick(Transform targetObject = null, string mode = "Click")
    {
        if (mode == "Click")
        {
            instance.pointer.SetActive(true);
            if (targetObject != null)
            {
                instance.pointer.transform.position = targetObject.position;
            }

            instance.pointer.GetComponent<Animator>().SetTrigger("Click");
        }

        if (mode == "SlideLeft")
        {
            instance.pointer.SetActive(true);
            instance.pointer.GetComponent<Animator>().SetTrigger("SlideLeft");
        }

        if (mode == "SlideRight")
        {
            instance.pointer.SetActive(true);
            instance.pointer.GetComponent<Animator>().SetTrigger("SlideRight");
        }
    }

    public void DeactivePointer()
    {
        pointer.SetActive(false);
    }

    #endregion
}