using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaptersHandler : MonoBehaviour
{
    public GameObject[] locks;

    public void UpdateChaptersState()
    {
        for (int i = 1; i < GameManager.instance.chapters.Length; i++)
        {
            if (ApplicationManager.instance.GetAllGemCount() >= GameManager.instance.chapters[i].cost)
                locks[i].SetActive(false);
            else
            {
                locks[i].SetActive(true);
                locks[i].GetComponentInChildren<Text>().text = GetCompeletionText(
                    ApplicationManager.instance.GetAllGemCount(),
                    GameManager.instance.chapters[i].cost);
            }
        }
    }

    private string GetCompeletionText(int current, int max)
    {
        string txt = (current < 10 ? "0" : "") + current + "/" + max;
        return txt;
    }

    internal void UnlockAll()
    {
        for (int i = 1; i < GameManager.instance.chapters.Length; i++)
        {
            locks[i].SetActive(false);
        }
    }
}