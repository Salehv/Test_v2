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
    public Vector2 messageLargeSize;
    public Vector2 messageMediumSize;
    public Vector2 messageSmallSize;

    internal GameObject currentMessage;
    private GameObject currentArrow;


    public void CreateMessage(string msg, Size size, Direction direction, GameObject destinationPanel,
        bool haveNextBtn = false, bool haveIcon = true)
    {
        currentMessage = Instantiate(tutMessagePrefab, destinationPanel.transform, true);
        currentMessage.GetComponentInChildren<RtlText>().text = msg;

        if (size == Size.Large)
        {
            currentMessage.GetComponent<RectTransform>().sizeDelta = messageLargeSize;
            currentMessage.transform.GetChild(2).transform.position = new Vector3(currentMessage.transform.GetChild(2).transform.position.x, -messageLargeSize.y/2-20);
            
        }

        if (size == Size.Medium)
        {
            currentMessage.GetComponent<RectTransform>().sizeDelta = messageMediumSize;
            currentMessage.transform.GetChild(2).transform.position = new Vector3(currentMessage.transform.GetChild(2).transform.position.x, -messageMediumSize.y/2-20);
        }

        if (size == Size.Small)
        {
            currentMessage.GetComponent<RectTransform>().sizeDelta = messageSmallSize;
            currentMessage.transform.GetChild(2).transform.position = new Vector3(currentMessage.transform.GetChild(2).transform.position.x, -messageSmallSize.y/2-20);
        }


        float messageX;

        if (haveIcon == false)
        {
            currentMessage.transform.GetChild(1).gameObject.SetActive(false);
            messageX = 0.45f;
        }
        else
        {
            currentMessage.transform.GetChild(1).gameObject.SetActive(true);
            messageX = 0.5f;
        }
        
        if (haveNextBtn == false)
        {
            currentMessage.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            currentMessage.transform.GetChild(2).gameObject.SetActive(true);
        }

        currentMessage.GetComponent<RectTransform>().localScale = Vector3.one;

        if (direction == Direction.Top)
        {
            RectTransform rt = currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.7f);
            rt.anchorMin = new Vector2(messageX, 0.7f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Topmost)
        {
            RectTransform rt = currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.8f);
            rt.anchorMin = new Vector2(messageX, 0.8f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Down)
        {
            RectTransform rt = currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.3f);
            rt.anchorMin = new Vector2(messageX, 0.3f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Downmost)
        {
            RectTransform rt = currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.13f);
            rt.anchorMin = new Vector2(messageX, 0.13f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Middle)
        {
            RectTransform rt = currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.5f);
            rt.anchorMin = new Vector2(messageX, 0.5f);
            rt.anchoredPosition = new Vector2(38, 0);
        }
    }

    public void DeleteMessage()
    {
        foreach (var animator in currentMessage.GetComponentsInChildren<Animator>())
        {
            animator.SetTrigger("Hide");
        }
    }

    public void DeleteMessageImmediate()
    {
        Destroy(currentMessage);
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

    public void DeleteArrow()
    {
        Destroy(currentArrow.gameObject);
    }

    public void ShineObjects(GameObject[] go)
    {
    }

    #endregion
    
    #region Pointer

    
    [Header("Pointer")]
    public GameObject pointer;

    public void ShowPointerClick(Transform targetObject = null, string mode = "")
    {
        if (mode == "" || mode == "Click")
        {
            pointer.SetActive(true);
            if (targetObject != null)
            {
                pointer.transform.position = targetObject.position;
            }

            pointer.GetComponent<Animator>().SetTrigger("Click");
        }
        if (mode == "SlideLeft")
        {
            pointer.SetActive(true);
            pointer.GetComponent<Animator>().SetTrigger("SlideLeft");
        }
        if (mode == "SlideRight")
        {
            pointer.SetActive(true);
            pointer.GetComponent<Animator>().SetTrigger("SlideRight");
        }
    }

    public void DeactivePointer()
    {
        pointer.SetActive(false);
    }

    #endregion

}