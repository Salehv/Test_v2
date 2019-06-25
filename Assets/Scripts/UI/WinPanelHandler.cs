using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelHandler : MonoBehaviour
{
    public GameObject[] gems;
    public Vector2[] pops;
    public Animator popAnim;
    public GameObject popPrefab;
    public Text coins;

    internal void Init(int gemsTaken, int coinsTaken)
    {
        foreach (var g in gems)
            g.SetActive(false);

        for (int i = 1; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        StartCoroutine(GemTaken(gemsTaken));
        coins.text = coinsTaken + "";
    }

    private IEnumerator GemTaken(int s)
    {
        switch (s)
        {
            case 1:
                AudioManager.instance.PlayNewSfx(SFX.GAME_WIN_1);
                popAnim.SetTrigger("1Gem");
                /*yield return new WaitForSeconds(0.2f);
                ShowGem(0);*/
                break;
            case 2:
                AudioManager.instance.PlayNewSfx(SFX.GAME_WIN_2);
                popAnim.SetTrigger("2Gem");
                /*yield return new WaitForSeconds(0.2f);
                ShowGem(0);
                yield return new WaitForSeconds(0.4f);
                ShowGem(1);*/
                break;
            case 3:
                AudioManager.instance.PlayNewSfx(SFX.GAME_WIN_3);
                popAnim.SetTrigger("3Gem");
                /*yield return new WaitForSeconds(0.2f);
                ShowGem(0);
                yield return new WaitForSeconds(0.52f);
                ShowGem(1);
                yield return new WaitForSeconds(0.85f);
                ShowGem(2);*/
                break;
            default:
                yield break;
        }
    }

    public void ShowGem(int id)
    {
        gems[id].SetActive(true);
        // GameObject g = Instantiate(popPrefab, transform);
        // (g.transform as RectTransform).anchoredPosition = pops[id];
    }
}