using System.Collections;
using System.Collections.Generic;
using App;
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
    TUT_02_02,
    TUT_01_07,
    TUT_01_08
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

        foreach (var page in TUT01_IntroPages)
        {
            page.SetActive(false);
        }

        foreach (GameObject introCharacter in Intro_Characters)
        {
            introCharacter.SetActive(false);
        }

        tut0102ELetters[0].transform.GetChild(1).GetComponent<Image>().sprite =
            GameManager.instance.GetLetterSprite(11, SpriteMode.NORMAL);

        tut0102ELetters[0].transform.GetChild(0).gameObject.SetActive(false);
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

    [Header("Intro")] public GameObject[] Intro_Characters;

    [Header("Tutorial 01")] public GameObject TUT01;
    public GameObject Tut01_MsgParent;
    public GameObject[] TUT01_IntroPages;

    [TextArea(1, 6)] public string[] TUT01_Texts;
    [Space(10)] public GameObject[] tut01Panels;

    public LetterPoolHandler letterPool;

    [Space(10)] public GameObject tut0101ALetter;
    public GameObject[] tut0102ELetters;
    public GameObject tut0103EndWord;


    public void PlayTutorial_01()
    {
        ResetAll();
        TUT01.SetActive(true);

        TimeManager.instance.SetTimer("IntroTimer");
        AnalyticsHandler.Intro_Started();

        TUT01_IntroPages[0].SetActive(true);
        Intro_Characters[0].SetActive(true);

        PopupHandler.CreateMessage(TUT01_Texts[0], Size.Medium, Direction.Topmost, Tut01_MsgParent, true, false);
        AudioManager.instance.PlayNewSfx(SFX.BLAH_1);

        AnalyticsHandler.Intro_Started();
        TimeManager.instance.SetTimer("IntroTimer");

        state = TutorialState.INTRO_01;

        PopupHandler.instance.DeactivePointer();
    }

    public void Intro_Clicked()
    {
        if (state != TutorialState.INTRO_01 && state != TutorialState.INTRO_02 &&
            state != TutorialState.INTRO_03) return;

        if (state == TutorialState.INTRO_01)
        {
            PopupHandler.DeleteMessage();
            PopupHandler.CreateMessage(TUT01_Texts[1], Size.Small, Direction.Downmost, Tut01_MsgParent, true, false);
            AudioManager.instance.PlayNewSfx(SFX.BLAH_2);


            Intro_Characters[0].GetComponent<Animator>().SetTrigger("hide");
            Intro_Characters[1].SetActive(true);

            state = TutorialState.INTRO_02;
            return;
        }

        if (state == TutorialState.INTRO_02)
        {
            PopupHandler.DeleteMessage();
            PopupHandler.CreateMessage(TUT01_Texts[2], Size.Medium, Direction.Topmost, Tut01_MsgParent, true, false);

            AudioManager.instance.PlayNewSfx(SFX.BLAH_1);
            Intro_Characters[0].SetActive(false);
            Intro_Characters[1].SetActive(false);
            Intro_Characters[2].SetActive(true);

            state = TutorialState.INTRO_03;
            return;
        }

        StartGameTutorial();
    }

    private void StartGameTutorial()
    {
        TUT01_IntroPages[0].SetActive(false);

        ViewManager.instance.ShowGameIntro();
        GameManager.instance.InitTutorial("ریشه", "زیره");


        PopupHandler.DeleteMessageImmediate();
        PopupHandler.CreateMessage(TUT01_Texts[3], Size.Small, Direction.Topmost);

        PopupHandler.ShowPointerClick(tut0101ALetter.transform);

        tut0101ALetter.SetActive(true);

        foreach (var letter in tut0102ELetters)
        {
            letter.SetActive(false);
        }

        state = TutorialState.TUT_01_01;
    }

    public void Tutorial_01_01_CharacterSelected()
    {
        if (state != TutorialState.TUT_01_01)
            return;

        PopupHandler.instance.DeactivePointer();
        PopupHandler.ShowPointerClick(tut0102ELetters[0].transform);

        // Turn off shine
        tut0101ALetter.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        tut0101ALetter.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite =
            GameManager.instance.GetLetterSprite(3, SpriteMode.SHINE);

        foreach (var letter in tut0102ELetters)
        {
            letter.SetActive(true);
        }

        tut0102ELetters[0].transform.GetChild(0).gameObject.SetActive(true);

        state = TutorialState.TUT_01_02;
    }

    public void Tutorial_01_02_EditCharacterSelected()
    {
        if (state != TutorialState.TUT_01_02)
            return;

        tut01Panels[0].GetComponent<Animator>().SetTrigger("fade");

        PopupHandler.instance.DeactivePointer();

        state = TutorialState.TUT_01_03;
    }

    public void Tutorial_01_FadedOut()
    {
        if (state != TutorialState.TUT_01_03)
            return;

        foreach (var letter in tut0102ELetters)
        {
            letter.SetActive(false);
        }

        PopupHandler.DeleteMessage();
        PopupHandler.CreateMessage(TUT01_Texts[4], Size.Small, Direction.Topmost, null, true);

        tut0101ALetter.SetActive(false);
        GameManager.instance.tutorialAcceptingWord = Utilities.GetNormalizedFarsi("تیشه");
        GameManager.instance.textEditor.ChangeLetterRequest(3, EditorHandler.IT_to_X(4, 0));

        state = TutorialState.TUT_01_04;
    }


    private bool tut01_hintNeeded = false;

    public void Tutorial_01_03_ScreenClicked()
    {
        if (state != TutorialState.TUT_01_04 && state != TutorialState.TUT_01_06 && state != TutorialState.TUT_01_08)
            return;

        if (state == TutorialState.TUT_01_04)
        {
            PopupHandler.DeleteMessage();
            PopupHandler.CreateMessage(TUT01_Texts[5], Size.Small, Direction.Top);

            GameViewManager.instance.SetEndWord(Utilities.GetNormalizedFarsi("تیره"));
            GameManager.instance.tutorialAcceptingWord = Utilities.GetNormalizedFarsi("تیره");

            tut01Panels[0].SetActive(false);

            state = TutorialState.TUT_01_05;
            tut01_hintNeeded = true;
            Invoke(nameof(ShowTut0104Help), 3);
            return;
        }

        if (state == TutorialState.TUT_01_06)
        {
            PopupHandler.DeleteMessage();
            PopupHandler.CreateMessage(TUT01_Texts[7], Size.Small, Direction.Top);


            tut01Panels[0].SetActive(false);
            state = TutorialState.TUT_01_07;
            return;
        }

        GameManager.instance.TutorialEnded();
        Tutorial_01_Completed();
    }

    internal void Tut01ALetterSelected(int code)
    {
        if (state == TutorialState.TUT_01_05)
        {
            if (code == 11)
            {
                tut01_hintNeeded = false;
                PopupHandler.instance.DeactivePointer();
                PopupHandler.ShowPointerClick(tut0102ELetters[2].transform);
            }

            else
            {
                tut01_hintNeeded = true;
                ShowTut0104Help();
            }
        }
    }

    private void ShowTut0104Help()
    {
        if (tut01_hintNeeded)
            PopupHandler.ShowPointerClick(letterPool.transform.GetChild(2));
    }


    public void Tutorial_01_WordCreated()
    {
        if (state != TutorialState.TUT_01_05 && state != TutorialState.TUT_01_07)
            return;

        if (state == TutorialState.TUT_01_05)
        {
            PopupHandler.instance.DeactivePointer();

            GameViewManager.instance.SetEndWord(Utilities.GetNormalizedFarsi("زیره"));
            GameManager.instance.tutorialAcceptingWord = Utilities.GetNormalizedFarsi("زیره");


            PopupHandler.DeleteMessage();
            PopupHandler.CreateMessage(TUT01_Texts[6], Size.Small, Direction.Top, null, true);

            tut01Panels[0].SetActive(true);
            foreach (var letter in tut0102ELetters)
            {
                letter.SetActive(false);
            }

            tut0101ALetter.SetActive(false);


            state = TutorialState.TUT_01_06;
            return;
        }


        PopupHandler.DeleteMessage();
        PopupHandler.CreateMessage(TUT01_Texts[8], Size.Small, Direction.Topmost, null, true, false);
        state = TutorialState.TUT_01_08;
        tut01Panels[0].SetActive(true);
    }


    public void Tutorial_01_Completed()
    {
        PopupHandler.DeleteMessage();
        ApplicationManager.instance.IntroEnded();

        int timeTaken = (int) TimeManager.instance.GetCurrentTime("IntroTimer");
        TimeManager.instance.DiscardTimer("IntroTimer");
        AnalyticsHandler.Intro_Finished(timeTaken);

        if (!PlayerPrefs.HasKey("MainMenuPointer"))
        {
            print("Main Menu Pointer Shown");
            PopupHandler.ShowPointerClick();
            PlayerPrefs.SetInt("MainMenuPointer", 1);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Tut02    

    [Header("Tutorial 2")] public GameObject tut02Object;

    [TextArea(0, 4)] public string[] tut02Messages;

    public GameObject tut0201Placeholder;
    public GameObject tut0202Undo;


    public void Play_Tutorial_02()
    {
        ResetAll();
        tut02Object.SetActive(true);

        PopupHandler.CreateMessage(tut02Messages[0], Size.Medium, Direction.Down, tut02Object);
        PopupHandler.instance.CreateArrow(tut0201Placeholder.transform, Direction.Top, tut02Object);

        state = TutorialState.TUT_02_01;
    }

    public void Tutorial_02_ScreenClicked()
    {
        if (state != TutorialState.TUT_02_01)
            return;

        PopupHandler.DeleteMessage();
        PopupHandler.DeleteArrow();

        PopupHandler.CreateMessage(tut02Messages[1], Size.Medium, Direction.Down, tut02Object);
        PopupHandler.instance.CreateArrow(tut0202Undo.transform, Direction.Top, tut02Object);

        tut0202Undo.SetActive(true);

        state = TutorialState.TUT_02_02;
    }

    public void Tutorial_02_UndoClicked()
    {
        PopupHandler.DeleteMessageImmediate();
        PopupHandler.DeleteArrow();

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
    public Button btnHintShowWay;
    public GameObject tut03_PopupNoMoney;
    public GameObject tut03_ShopPanel;

    public void Play_Tutorial_03()
    {
        ResetAll();
        TUT03.SetActive(true);
        PopupHandler.CreateMessage("حالا وقتشه که از راهنمایی استفاده کنی!", Size.Small, Direction.Topmost, TUT03);
        state = TutorialState.TUT_03_01;
    }

    public void Tutorial_03_ScreenClicked()
    {
        if (state != TutorialState.TUT_03_01) return;

        PopupHandler.DeleteMessage();
        PopupHandler.CreateMessage("آیکون راهنمایی رو لمس کن", Size.Small, Direction.Topmost, TUT03);
        PopupHandler.instance.CreateArrow(GameManager.instance.btnHint.transform, Direction.Top, TUT03);

        tut03_BtnHint.gameObject.SetActive(true);

        state = TutorialState.TUT_03_02;
    }

    public void Tutorial_03_HintClicked()
    {
        if (state != TutorialState.TUT_03_02) return;

        PopupHandler.DeleteMessage();
        PopupHandler.DeleteArrow();
        PopupHandler.CreateMessage("خب! حالا اگه این گزینه رو انتخاب کنی کل مسیر رسیدن به کلمه هدف رو می بینی",
            Size.Medium,
            Direction.Downmost, TUT03);
        PopupHandler.instance.CreateArrow(btnHintShowWay.transform, Direction.Right, TUT03);

        tut03_HintPanel.SetActive(true);

        state = TutorialState.TUT_03_03;
    }

    public void Tutorial_03_HintShowWayClicked()
    {
        if (state != TutorialState.TUT_03_03) return;

        PopupHandler.DeleteMessage();
        PopupHandler.DeleteArrow();
        PopupHandler.CreateMessage("با ورود به فروشگاه می تونی خیلی سریع سکه بخری!", Size.Small, Direction.Downmost,
            TUT03);
        PopupHandler.instance.CreateArrow(tut03_BtnShop.transform, Direction.Right, TUT03, new Vector2(150, 0));

        tut03_HintPanel.SetActive(false);
        tut03_PopupNoMoney.SetActive(true);

        state = TutorialState.TUT_03_04;
    }

    public void Tutorial_03_ShopClicked()
    {
        if (state != TutorialState.TUT_03_04) return;

        PopupHandler.DeleteMessage();
        PopupHandler.DeleteArrow();
        PopupHandler.CreateMessage("این گزینه رو لمس کن تا مجانی سکه بگیری", Size.Small, Direction.Middle, TUT03);
        PopupHandler.instance.CreateArrow(tut03_BtnBuy.transform, Direction.Right, TUT03);


        tut03_PopupNoMoney.SetActive(false);
        tut03_ShopPanel.SetActive(true);

        state = TutorialState.TUT_03_05;
    }

    public void Tutorial_03_BuyClicked()
    {
        if (state != TutorialState.TUT_03_05) return;

        PopupHandler.DeleteMessage();
        PopupHandler.DeleteArrow();
        PopupHandler.CreateMessage("حالا که سکه خریدی می تونی راهنمایی رو ببینی", Size.Small, Direction.Middle,
            TUT03);

        state = TutorialState.TUT_03_06;
    }

    public void Tutorial_03_ShowHint()
    {
        if (state != TutorialState.TUT_03_06) return;

        Destroy(PopupHandler.instance.currentMessage);
        PopupHandler.DeleteArrow();
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

        PopupHandler.CreateMessage(tut04Messages[0], Size.Medium, Direction.Downmost, tut04);
        PopupHandler.instance.CreateArrow(tut0401Letter.transform, Direction.Right, tut04, new Vector3(20, 0));


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

        PopupHandler.DeleteMessage();
        PopupHandler.DeleteArrow();
        PopupHandler.CreateMessage(tut04Messages[1], Size.Small, Direction.Downmost, tut04);
        PopupHandler.instance.CreateArrow(tut0402Pluses[1].transform, Direction.Top, tut04, new Vector3(0, 100));

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

        PopupHandler.DeleteMessage();
        PopupHandler.DeleteArrow();

        PopupHandler.CreateMessage(tut04Messages[2], Size.Large, Direction.Down, tut04);

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
        PopupHandler.DeleteMessageImmediate();
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
        PopupHandler.CreateMessage(tut05Messages[0], Size.Medium, Direction.Down, tut05);
        PopupHandler.instance.CreateArrow(tut05ELetters[0].transform, Direction.Top, tut05);

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

        PopupHandler.DeleteMessage();
        PopupHandler.CreateMessage(tut05Messages[1], Size.Medium, Direction.Down, tut05);

        PopupHandler.DeleteArrow();
        PopupHandler.instance.CreateArrow(tut05ELetters[0].transform.GetChild(0), Direction.Left, tut05,
            new Vector2(-10, 130));

        tut05ELetters[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("show");
    }

    public void Tutorial_05_Delete()
    {
        if (state != TutorialState.TUT_05_02)
            return;

        PopupHandler.DeleteMessage();
        PopupHandler.CreateMessage(tut05Messages[2], Size.Medium, Direction.Down, tut05);

        PopupHandler.DeleteArrow();


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

        PopupHandler.DeleteMessageImmediate();
        tut05.SetActive(false);
        GameManager.instance.TutorialEnded();
        AnalyticsHandler.Tutorial_Completed05();
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