using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LetterPoolHandler : MonoBehaviour
{
    [SerializeField] protected GameObject letterPrefab;

    public float cellWidth;
    public float topPadding;
    public float leftPadding;
    public Vector2 spacing;

    internal void Init(EditorHandler editor, int[] chars)
    {
        // Remove all childs
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        List<int> sortedChars = new List<int>(chars);
        sortedChars.Sort();

        for (int i = 0; i < 9; i++)
        {
            CreateAvailableLetter(editor, sortedChars[i], i);
        }


        // RebuildLayout();
    }


    private void CreateAvailableLetter(EditorHandler editor, int letter, int index)
    {
        GameObject g = Instantiate(letterPrefab, this.GetComponent<Transform>(), true);
        // g.GetComponent<Image>().sprite = null;
        g.transform.localPosition = Vector3.zero;

        try
        {
            g.GetComponent<AvailableLetter>().Init(editor, GameManager.dic_letterToChar[letter]);
        }
        catch (KeyNotFoundException e)
        {
            print(letter + " " + e.Message);
        }

        // Debug.Log("[[ Letter: " + letter + " ]]");
    }

    private void RebuildLayout()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var rt = transform.GetChild(i) as RectTransform;

            rt.anchorMax = new Vector2(1, 1);
            rt.anchorMin = new Vector2(1, 1);

            rt.anchoredPosition =
                new Vector2(-(i % 3) * (cellWidth + spacing.x) - leftPadding,
                    -((int) (i / 3) * (cellWidth + spacing.y) + topPadding)) +
                new Vector2(cellWidth / 2, cellWidth / 2);
            rt.sizeDelta = new Vector2(cellWidth, cellWidth);
            rt.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnValidate()
    {
        RebuildLayout();
    }

    private void OnTransformChildrenChanged()
    {
        RebuildLayout();
    }
}