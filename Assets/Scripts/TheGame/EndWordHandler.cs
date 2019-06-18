using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndWordHandler : MonoBehaviour
{
    public GameObject endLetterPrefab;
    public Transform contents;

    internal void CreateLetter(char chr)
    {
        GameObject g = Instantiate(endLetterPrefab, contents, true);
        g.GetComponentsInChildren<Image>()[1].sprite =
            GameManager.instance.GetLetterSprite(Utilities.dic_charToLetter[chr], SpriteMode.GOLD);
        g.transform.localScale = Vector3.one;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(contents as RectTransform);
    }
}