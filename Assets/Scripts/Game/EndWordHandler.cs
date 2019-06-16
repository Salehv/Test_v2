using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndWordHandler : MonoBehaviour
{
    internal static EndWordHandler instance;
    public GameObject letterPrefab;

    private void Awake()
    {
        instance = this;
    }

    internal void CreateLetter(char chr)
    {
        GameObject g = Instantiate(letterPrefab, transform, true);
        g.GetComponentsInChildren<Image>()[1].sprite =
            GameManager.instance.GetLetterSprite(GameManager.dic_charToLetter[chr], SpriteMode.GOLD);
        g.transform.localScale = Vector3.one;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}