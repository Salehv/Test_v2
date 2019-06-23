using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK.Setup;
using TheGame;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;
using UPersian.Components;

public enum TutorialState
{
    INTRO_01,
    INTRO_02,
    INTRO_03,
    INTRO_04,
    INTRO_05,
    INTRO_06,
    INTRO_07,
    TUT_01_01,
    TUT_01_02,
    TUT_01_03,
    TUT_01_04,
    TUT_01_05,
    TUT_01_06,
    TUT_02_01,
    TUT_03_01,
    TUT_03_02,
    TUT_03_03,
    TUT_03_04,
    TUT_03_05,
    TUT_03_06,
    TUT_03_PASSED,
    TUT_04_01,
    TUT_04_02,
    TUT_04_03,
    TUT_05_01,
    TUT_05_02,
    TUT_05_03,
    TUT_02_02
}

public class TutorialHandler : MonoBehaviour
{
    public static TutorialHandler instance;
    public TutorialState state;

    private void Awake()
    {
        instance = this;
    }


    #region Tutorial Reset

    internal void ResetAll()
    {
        // TUT01
        ResetTut01();

        // TUT02
        ResetTut02();

        // TUT03
        ResetTut03();

        // TUT04
        ResetTut04();

        // TUT05
        ResetTut05();
    }

    private void ResetTut01()
    {
        TUT01.SetActive(false);

        foreach (var arrow in TUT01_Arrows)
        {
            arrow.SetActive(false);
        }

        TUT01_03_LetterShine.SetActive(false);

        foreach (var page in TUT01_IntroPages)
        {
            page.SetActive(false);
        }

        TutELetter.selected = 25;
        TUT01_05_Letter.GetComponent<TutELetter>().ChangeLetter();
        TutELetter.selected = 26;
        TUT01_03_Letter.GetComponent<TutELetter>().ChangeLetter();
    }

    private void ResetTut02()
    {
        tut0201Placeholder.SetActive(true);

        tut02Object.SetActive(false);
        tut0202Undo.SetActive(false);
    }

    private void ResetTut03()
    {
        tut03_BtnHint.gameObject.SetActive(false);
        tut03_HintPanel.SetActive(false);
        tut03_PopupNoMoney.SetActive(false);
        tut03_ShopPanel.SetActive(false);
        TUT03.SetActive(false);
    }

    private void ResetTut04()
    {
        tut04.SetActive(false);

        foreach (var plus in tut0402Pluses) // Pluses
        {
            plus.SetActive(false);
        }

        foreach (var letter in tut04ELetters) // Eletters
        {
            letter.SetActive(false);
        }

        tut0401Letter.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = // Letter
            GameManager.instance.GetLetterSprite(0, SpriteMode.NORMAL);

        tut0401Letter.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        tut0401Letter.SetActive(false);
    }


    private void ResetTut05()
    {
        tut05.SetActive(false);

        foreach (var letter in tut05ELetters)
        {
            letter.SetActive(true);
        }
    }

    #endregion


    #region Tut01

    [Header("Tutorial 01")] public GameObject TUT01;
    public GameObject Tut01_MsgParent;
    public GameObject[] TUT01_Arrows;
    public GameObject[] TUT01_IntroPages;
    [TextArea(1, 6)] public string[] TUT01_Texts;

    [Header("Tutorial 01_03")] public GameObject TUT01_03_LetterShine;
    public Animator TUT01_03_Letter;
    public Image TUT01_03_ALetterImage;

    [Header("Tutorial 01_05")] public Animator TUT01_05_Letter;
    public GameObject TUT01_05_Shine;
    public Image TUT01_05_ALetterImage;

    public void PlayTutorial_01()
    {
        ResetAll();
        TUT01.SetActive(true);

        TimeManager.instance.SetTimer("IntroTimer");
        AnalyticsHandler.Intro_Started();

        Tut01_MsgParent.GetComponent<Image>().raycastTarget = false;
        TUT01_IntroPages[0].SetActive(true);


        CreateMessage(TUT01_Texts[0], Size.Medium, Direction.Topmost, Tut01_MsgParent, false);

        AnalyticsHandler.Intro_Started();
        TimeManager.instance.SetTimer("IntroTimer");

        state = TutorialState.INTRO_01;
    }

    public void Intro_01()
    {
        if (state != TutorialState.INTRO_01) return;

        TUT01_IntroPages[0].SetActive(false);
        TUT01_IntroPages[1].SetActive(true);

        DeleteMessageImmediate();

        CreateMessage(TUT01_Texts[1], Size.Small, Direction.Downmost, Tut01_MsgParent, false);

        state = TutorialState.INTRO_02;
    }

    public void Intro_02()
    {
        if (state != TutorialState.INTRO_02) return;

        TUT01_IntroPages[1].SetActive(false);
        TUT01_IntroPages[2].SetActive(true);

        DeleteMessageImmediate();
        CreateMessage(TUT01_Texts[2], Size.Medium, Direction.Topmost, Tut01_MsgParent, false);

        state = TutorialState.INTRO_03;
    }

    public void Intro_03()
    {
        if (state != TutorialState.INTRO_03) return;

        Tut01_MsgParent.GetComponent<Image>().raycastTarget = true;
        TUT01_IntroPages[2].SetActive(false);
        TUT01_IntroPages[3].SetActive(true);

        DeleteMessageImmediate();
        CreateMessage(TUT01_Texts[3], Size.Large, Direction.Topmost, Tut01_MsgParent, true);

        state = TutorialState.TUT_01_01;
    }

    public void Tutorial_01_ScreenClicked()
    {
        if (state != TutorialState.TUT_01_01 && state != TutorialState.TUT_01_06)
            return;

        if (state == TutorialState.TUT_01_01)
        {
            state = TutorialState.TUT_01_02;

            TUT01_Arrows[0].SetActive(false);
            TUT01_Arrows[1].SetActive(true);

            DeleteMessage();
            CreateMessage(TUT01_Texts[4], Size.Small, Direction.Topmost, Tut01_MsgParent);

            Tut01_MsgParent.GetComponent<Image>().raycastTarget = false;
            return;
        }

        //Tutorial_01_Completed();
        Intro_04();
    }


    public void Tutoial_01_FirstCharClicked()
    {
        if (state != TutorialState.TUT_01_02)
            return;

        TUT01_03_ALetterImage.sprite = GameManager.instance.GetLetterSprite(12, SpriteMode.SHINE);
        TUT01_03_LetterShine.SetActive(true);

        TUT01_Arrows[1].SetActive(false);
        TUT01_Arrows[2].SetActive(true);

        DeleteMessage();
        CreateMessage(TUT01_Texts[5], Size.Medium, Direction.Topmost, Tut01_MsgParent);

        state = TutorialState.TUT_01_03;
    }

    public void Tutorial_01_EditCharacter01Clicked()
    {
        if (state != TutorialState.TUT_01_03)
            return;

        TutELetter.selected = 12;
        TUT01_03_Letter.SetTrigger("rotate");
        TUT01_Arrows[2].SetActive(false);
        TUT01_Arrows[3].SetActive(true);

        DeleteMessage();
        CreateMessage(TUT01_Texts[6], Size.Medium, Direction.Topmost, Tut01_MsgParent);

        TUT01_03_ALetterImage.sprite = GameManager.instance.GetLetterSprite(12, SpriteMode.NORMAL);

        TUT01_03_LetterShine.SetActive(false);

        state = TutorialState.TUT_01_04;
        AnalyticsHandler.Intro_FirstCharacterChanged();
    }


    public void Tutorial_01_SecondCharacter()
    {
        if (state != TutorialState.TUT_01_04)
            return;

        DeleteMessage();
        CreateMessage(TUT01_Texts[7], Size.Small, Direction.Topmost, Tut01_MsgParent);

        TUT01_Arrows[3].SetActive(false);
        TUT01_Arrows[4].SetActive(true);

        TUT01_05_Shine.SetActive(true);
        TUT01_05_ALetterImage.sprite = GameManager.instance.GetLetterSprite(11, SpriteMode.SHINE);

        state = TutorialState.TUT_01_05;
    }

    public void Tutorial_01_EditCharacter02Clicked()
    {
        if (state != TutorialState.TUT_01_05)
            return;

        TutELetter.selected = 11;
        TUT01_05_Letter.SetTrigger("rotate");
        DeleteMessage();
        CreateMessage(TUT01_Texts[8], Size.Large, Direction.Topmost, Tut01_MsgParent);
        TUT01_Arrows[4].SetActive(false);
        TUT01_05_Shine.SetActive(false);
        TUT01_05_ALetterImage.sprite = GameManager.instance.GetLetterSprite(11, SpriteMode.NORMAL);
        Tut01_MsgParent.GetComponent<Image>().raycastTarget = true;
        state = TutorialState.TUT_01_06;
        AnalyticsHandler.Intro_SecondCharacterChanged();
    }


    public void Intro_04()
    {
        if (state != TutorialState.TUT_01_06) return;

        TUT01_IntroPages[3].SetActive(false);
        TUT01_IntroPages[4].SetActive(true);

        Tut01_MsgParent.GetComponent<Image>().raycastTarget = false;

        DeleteMessage();
        CreateMessage(TUT01_Texts[9], Size.Medium, Direction.Topmost, Tut01_MsgParent, false);

        state = TutorialState.INTRO_04;
    }


    public void Tutorial_01_Completed()
    {
        ApplicationManager.instance.IntroEnded();

        int timeTaken = (int) TimeManager.instance.GetCurrentTime("IntroTimer");
        TimeManager.instance.DiscardTimer("IntroTimer");
        AnalyticsHandler.Intro_Finished(timeTaken);
    }


    public void ExitTutorial_01()
    {
        TUT01.SetActive(false);
    }

    #endregion

    #region Tut02

    [FormerlySerializedAs("TUT02")] [Space(30)] [Header("Tutorial 2")]
    public GameObject tut02Object;

    [TextArea(0, 4)] public string[] tut02Messages;

    public GameObject tut0201Placeholder;
    public GameObject tut0202Undo;


    public void Play_Tutorial_02()
    {
        ResetAll();
        tut02Object.SetActive(true);

        CreateMessage(tut02Messages[0], Size.Medium, Direction.Down, tut02Object);
        CreateArrow(tut0201Placeholder.transform, Direction.Top, tut02Object);

        state = TutorialState.TUT_02_01;
    }

    public void Tutorial_02_ScreenClicked()
    {
        if (state != TutorialState.TUT_02_01)
            return;

        DeleteMessage();
        DeleteArrow();

        CreateMessage(tut02Messages[1], Size.Medium, Direction.Down, tut02Object);
        CreateArrow(tut0202Undo.transform, Direction.Top, tut02Object);

        tut0202Undo.SetActive(true);

        state = TutorialState.TUT_02_02;
    }

    public void Tutorial_02_UndoClicked()
    {
        DeleteMessageImmediate();
        DeleteArrow();

        GameManager.instance.Undo();
        tut02Object.SetActive(false);

        GameManager.instance.TutorialEnded();

        AnalyticsHandler.Tutorial_Completed02();
    }

    #endregion

    #region Tut03

    [Space(30)] [Header("Tutorial 03")] public GameObject TUT03;
    public Button tut03_BtnHint;
    public Button tut03_BtnShop;
    public Button tut03_BtnBuy;
    public GameObject tut03_HintPanel;
    public GameObject tut03_PopupNoMoney;
    public GameObject tut03_ShopPanel;

    public void Play_Tutorial_03()
    {
        ResetAll();
        TUT03.SetActive(true);
        CreateMessage("حالا وقتشه که از راهنمایی استفاده کنی!", Size.Small, Direction.Topmost, TUT03);
        state = TutorialState.TUT_03_01;
    }

    public void Tutorial_03_ScreenClicked()
    {
        if (state != TutorialState.TUT_03_01) return;

        DeleteMessage();
        CreateMessage("آیکون راهنمایی رو لمس کن", Size.Small, Direction.Topmost, TUT03);
        CreateArrow(GameManager.instance.btnHint.transform, Direction.Top, TUT03);

        tut03_BtnHint.gameObject.SetActive(true);

        state = TutorialState.TUT_03_02;
    }

    public void Tutorial_03_HintClicked()
    {
        if (state != TutorialState.TUT_03_02) return;

        DeleteMessage();
        DeleteArrow();
        CreateMessage("خب! حالا اگه این گزینه رو انتخاب کنی کل مسیر رسیدن به کلمه هدف رو می بینی", Size.Medium,
            Direction.Downmost, TUT03);
        CreateArrow(GameManager.instance.btnHintShowWay.transform, Direction.Right, TUT03);

        tut03_HintPanel.SetActive(true);

        state = TutorialState.TUT_03_03;
    }

    public void Tutorial_03_HintShowWayClicked()
    {
        if (state != TutorialState.TUT_03_03) return;

        DeleteMessage();
        DeleteArrow();
        CreateMessage("با ورود به فروشگاه می تونی خیلی سریع سکه بخری!", Size.Small, Direction.Downmost, TUT03);
        CreateArrow(tut03_BtnShop.transform, Direction.Right, TUT03, new Vector2(150, 0));

        tut03_HintPanel.SetActive(false);
        tut03_PopupNoMoney.SetActive(true);

        state = TutorialState.TUT_03_04;
    }

    public void Tutorial_03_ShopClicked()
    {
        if (state != TutorialState.TUT_03_04) return;

        DeleteMessage();
        DeleteArrow();
        CreateMessage("این گزینه رو لمس کن تا مجانی سکه بگیری", Size.Small, Direction.Middle, TUT03);
        CreateArrow(tut03_BtnBuy.transform, Direction.Right, TUT03);


        tut03_PopupNoMoney.SetActive(false);
        tut03_ShopPanel.SetActive(true);

        state = TutorialState.TUT_03_05;
    }

    public void Tutorial_03_BuyClicked()
    {
        if (state != TutorialState.TUT_03_05) return;

        DeleteMessage();
        DeleteArrow();
        CreateMessage("حالا که سکه خریدی می تونی راهنمایی رو ببینی", Size.Small, Direction.Middle,
            TUT03);

        state = TutorialState.TUT_03_06;
    }

    public void Tutorial_03_ShowHint()
    {
        if (state != TutorialState.TUT_03_06) return;

        Destroy(_currentMessage);
        DeleteArrow();
        ResetAll();

        state = TutorialState.TUT_03_PASSED;
        GameManager.instance.TutorialEnded();

        GameManager.instance.AddCoins(GameManager.instance.showWayHintCost);
        GameManager.instance.Hint_ShowWay();

        AnalyticsHandler.Tutorial_Completed03();
    }

    #endregion

    #region Tut04

    [Space(30)] [Header("Tutorial 04")] public GameObject tut04;
    [TextArea(0, 4)] public string[] tut04Messages;
    public GameObject tut0401Letter;
    public GameObject[] tut04ELetters;
    public GameObject[] tut0402Pluses;


    public void Play_Tutorial_04()
    {
        ResetAll();
        tut04.SetActive(true);

        CreateMessage(tut04Messages[0], Size.Medium, Direction.Downmost, tut04);
        CreateArrow(tut0401Letter.transform, Direction.Right, tut04, new Vector3(20, 0));


        tut0401Letter.SetActive(true);
        foreach (var letter in tut04ELetters)
        {
            letter.SetActive(true);
        }

        state = TutorialState.TUT_04_01;
    }

    public void Tutorial04_LetterClicked()
    {
        if (state != TutorialState.TUT_04_01)
            return;

        DeleteMessage();
        DeleteArrow();
        CreateMessage(tut04Messages[1], Size.Small, Direction.Downmost, tut04);
        CreateArrow(tut0402Pluses[1].transform, Direction.Top, tut04, new Vector3(0, 100));

        // Shine
        tut0401Letter.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite =
            GameManager.instance.GetLetterSprite(0, SpriteMode.SHINE);
        tut0401Letter.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        // Active Pluses
        foreach (var plus in tut0402Pluses)
        {
            plus.SetActive(true);
        }

        state = TutorialState.TUT_04_02;
    }

    public void Tutorial04_PlusClicked()
    {
        if (state != TutorialState.TUT_04_02)
            return;

        GameManager.instance.AddLetter(1, 0);
        GameManager.instance.textEditor.Correct();

        DeleteMessage();
        DeleteArrow();

        CreateMessage(tut04Messages[2], Size.Large, Direction.Down, tut04);

        // Hide E Letters
        foreach (var letter in tut04ELetters)
        {
            letter.SetActive(false);
        }


        tut0401Letter.SetActive(false);


        // UnShine
        tut0401Letter.transform.GetChild(0).GetComponent<Image>().sprite =
            GameManager.instance.GetLetterSprite(0, SpriteMode.NORMAL);

        tut0401Letter.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        // Hide Pluses
        foreach (var plus in tut0402Pluses)
        {
            plus.SetActive(false);
        }

        state = TutorialState.TUT_04_03;
    }

    public void Tutorial04_ScreenClicked()
    {
        if (state != TutorialState.TUT_04_03)
            return;
        DeleteMessageImmediate();
        tut04.SetActive(false);
        GameManager.instance.TutorialEnded();
        AnalyticsHandler.Tutorial_Completed04();
    }

    #endregion

    #region Tut05

    [Space(30)] [Header("Tutorial 05")] public GameObject tut05;
    [TextArea(0, 4)] public string[] tut05Messages;

    public GameObject[] tut05ELetters;


    public void Play_Tutorial_05()
    {
        ResetAll();


        tut05.SetActive(true);
        CreateMessage(tut05Messages[0], Size.Medium, Direction.Down, tut05);
        CreateArrow(tut05ELetters[0].transform, Direction.Top, tut05);

        foreach (var letter in tut05ELetters)
        {
            letter.SetActive(true);
        }

        tut05ELetters[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("hide");

        state = TutorialState.TUT_05_01;
    }

    public void Tutorial_05_ELetterClicked()
    {
        if (state != TutorialState.TUT_05_01)
            return;

        state = TutorialState.TUT_05_02;

        DeleteMessage();
        CreateMessage(tut05Messages[1], Size.Medium, Direction.Down, tut05);

        DeleteArrow();
        CreateArrow(tut05ELetters[0].transform.GetChild(0), Direction.Left, tut05, new Vector2(-10, 130));

        tut05ELetters[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("show");
    }

    public void Tutorial_05_Delete()
    {
        if (state != TutorialState.TUT_05_02)
            return;

        DeleteMessage();
        CreateMessage(tut05Messages[2], Size.Medium, Direction.Down, tut05);

        DeleteArrow();


        foreach (var letter in tut05ELetters)
        {
            letter.SetActive(false);
        }

        GameManager.instance.RemoveLetter(0);
        GameManager.instance.textEditor.RemoveLetter(11);
        GameManager.instance.textEditor.Correct();

        state = TutorialState.TUT_05_03;
    }

    public void Tutorial_05_ScreenClicked()
    {
        if (state != TutorialState.TUT_05_03)
            return;

        DeleteMessageImmediate();
        tut05.SetActive(false);
        GameManager.instance.TutorialEnded();
        AnalyticsHandler.Tutorial_Completed05();
    }

    #endregion

    #region TutorialAutomation

    [Header("Tutorial Automation")] public GameObject tutMessagePrefab;
    public GameObject arrowTut;
    public Vector2 messageLargeSize;
    public Vector2 messageMediumSize;
    public Vector2 messagesmallSize;

    private GameObject _currentMessage;
    private GameObject _currentArrow;


    public void CreateMessage(string msg, Size size, Direction direction, GameObject destinationPanel,
        bool haveIcon = true)
    {
        _currentMessage = Instantiate(tutMessagePrefab, destinationPanel.transform, true);
        _currentMessage.GetComponentInChildren<RtlText>().text = msg;

        if (size == Size.Large)
        {
            _currentMessage.GetComponent<RectTransform>().sizeDelta = messageLargeSize;
        }

        if (size == Size.Medium)
        {
            _currentMessage.GetComponent<RectTransform>().sizeDelta = messageMediumSize;
        }

        if (size == Size.Small)
        {
            _currentMessage.GetComponent<RectTransform>().sizeDelta = messagesmallSize;
        }


        float messageX;

        if (haveIcon == false)
        {
            _currentMessage.transform.GetChild(1).gameObject.SetActive(false);
            messageX = 0.45f;
        }

        else
        {
            _currentMessage.transform.GetChild(1).gameObject.SetActive(true);
            messageX = 0.5f;
        }

        _currentMessage.GetComponent<RectTransform>().localScale = Vector3.one;

        if (direction == Direction.Top)
        {
            RectTransform rt = _currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.7f);
            rt.anchorMin = new Vector2(messageX, 0.7f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Topmost)
        {
            RectTransform rt = _currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.8f);
            rt.anchorMin = new Vector2(messageX, 0.8f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Down)
        {
            RectTransform rt = _currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.3f);
            rt.anchorMin = new Vector2(messageX, 0.3f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Downmost)
        {
            RectTransform rt = _currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.13f);
            rt.anchorMin = new Vector2(messageX, 0.13f);
            rt.anchoredPosition = new Vector2(38, 0);
        }

        if (direction == Direction.Middle)
        {
            RectTransform rt = _currentMessage.transform as RectTransform;
            rt.anchorMax = new Vector2(messageX, 0.5f);
            rt.anchorMin = new Vector2(messageX, 0.5f);
            rt.anchoredPosition = new Vector2(38, 0);
        }
    }

    public void DeleteMessage()
    {
        foreach (var animator in _currentMessage.GetComponentsInChildren<Animator>())
        {
            animator.SetTrigger("Hide");
        }
    }

    public void DeleteMessageImmediate()
    {
        Destroy(_currentMessage);
    }


    public void CreateArrow(Transform objTransform, Direction direction, GameObject destinationPanel,
        Vector2 offset = default(Vector2))
    {
        _currentArrow = Instantiate(arrowTut, destinationPanel.transform);
        _currentArrow.transform.position = objTransform.position;
        (_currentArrow.transform as RectTransform).anchoredPosition += offset;

        if (direction == Direction.Top)
        {
            _currentArrow.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        if (direction == Direction.Left)
        {
            _currentArrow.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        if (direction == Direction.Down)
        {
            _currentArrow.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }

    public void DeleteArrow()
    {
        Destroy(_currentArrow.gameObject);
    }

    public void ShineObjects(GameObject[] go)
    {
    }

    #endregion

    public void Escape()
    {
    }
}

public enum Direction
{
    Top,
    Left,
    Right,
    Down,
    Middle,
    Topmost,
    Downmost
}

public enum Size
{
    Small,
    Medium,
    Large
}