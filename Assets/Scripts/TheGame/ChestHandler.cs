using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using App;
using TheGame;
using UnityEngine;
using UPersian.Components;

public class ChestHandler : MonoBehaviour
{
    public GameObject chest, gem, coin;
    public static ChestHandler instance;


    public void CallChest(int coinAmount, int gemAmount, bool isEndOfChapter = false, int chapterToUnlock = 0)
    {
        if (coinAmount < 0 || gemAmount < 0)
            return;

        chest.SetActive(true);
        coin.GetComponentInChildren<RtlText>().text = "+" + coinAmount;
        gem.GetComponentInChildren<RtlText>().text = "+" + gemAmount;

        if (coinAmount == 0)
        {
            coin.SetActive(false);
            (gem.transform as RectTransform).anchoredPosition = new Vector2(0, 348.9f);
        }

        if (gemAmount == 0)
        {
            gem.SetActive(false);
            (coin.transform as RectTransform).anchoredPosition = new Vector2(0, 348.9f);
        }

        GameManager.instance.AddCoins(coinAmount);
        GameManager.instance.AddGems(gemAmount);

        AnalyticsHandler.ChestOpened(coinAmount, gemAmount);

        if (isEndOfChapter)
        {
            _isInChapterEndCinematic = true;
            _targetChapter = chapterToUnlock;
        }
    }


    private bool _isInChapterEndCinematic = false;
    private int _targetChapter;

    public void Accept()
    {
        if (_isInChapterEndCinematic)
        {
            ViewManager.instance.Escape();

            ApplicationManager.instance.chapterScroller.GoToPageDelayed(_targetChapter, 1f);
        }

        chest.SetActive(false);
    }

    public void Start()
    {
        instance = this;
    }
}