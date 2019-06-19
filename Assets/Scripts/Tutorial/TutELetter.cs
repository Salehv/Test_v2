using System.Collections;
using System.Collections.Generic;
using TheGame;
using UnityEngine;
using UnityEngine.UI;

public class TutELetter : MonoBehaviour
{
    public static int selected;

    public Image letter;

    public void ChangeLetter()
    {
        letter.sprite = GameManager.instance.GetLetterSprite(selected, SpriteMode.NORMAL);
    }
}