using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelObject : MonoBehaviour
{
    [SerializeField] public Level level;

    [Space(10)] public Text levelID;

    public Image background;
    
    public Text lockCountDownTime;

    public GameObject[] gems;

    internal bool isUnlicked;


    public void LevelClicked()
    {
        TransitionHandler.instance.StartTransition(level);
    }


    internal void Init()
    {
        levelID.text = (level.id + 1) + "";
        background.sprite = ResourceManager.instance.GetChapterLevelMenuSprite(level.chapterId);

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
}