using System;
using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;
using UnityEngine.UI;

public class LevelObject : MonoBehaviour
{
    public Level level;

    [Space(10)] 
    public Text levelId;

    public Image background;
    public Text lockCountDownTime;
    
    public GameObject lockImage;
    public GameObject gemHolder;
    public GameObject[] gems;

    internal bool isUnlocked;

    public void LevelClicked()
    {
        ViewManager.instance.LevelClicked(level);
    }

    internal void Init()
    {
        levelId.text = (level.id + 1) + "";
        background.sprite = ResourceManager.instance.GetLevelSprite(level.chapterId);
        
        if (gems.Length == 3)
        {
            gems[0].SetActive(true);
            gems[1].SetActive(true);
            gems[2].SetActive(true);

            switch (level.gems)
            {
                case 0:
                    gems[0].SetActive(false);
                    gems[1].SetActive(false);
                    gems[2].SetActive(false);
                    break;

                case 1:
                    gems[1].SetActive(false);
                    gems[2].SetActive(false);
                    break;

                case 2:
                    gems[2].SetActive(false);
                    break;
                case 3:
                    break;
            }
        }
    }

    internal void SetLock()
    {
        lockImage.SetActive(true);
        levelId.gameObject.SetActive(false);
        gemHolder.SetActive(false);
        background.color = new Color(0.6f, 0.6f, 0.6f);
    }

    internal void SetSolved()
    {
        lockImage.SetActive(false);
        levelId.gameObject.SetActive(true);
        gemHolder.SetActive(true);
        background.color = new Color(0.85f, 0.85f, 0.85f);
    }

    internal void SetUnSolved()
    {
        lockImage.SetActive(false);
        levelId.gameObject.SetActive(true);
        gemHolder.SetActive(false);
        background.color = new Color(1, 1, 1);
    }
}