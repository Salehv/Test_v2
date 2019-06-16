using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QF_RateHandler : MonoBehaviour
{
    public Button[] rateStars;
    private int currentRate = -1;

    private void Start()
    {
        foreach (Button t in rateStars)
        {
            t.onClick.AddListener(delegate { toggleChanged(t.transform.GetSiblingIndex()); });
        }
    }


    public void toggleChanged(int rank)
    {
        print(rank);
        for (int i = 0; i <= rank; i++)
            rateStars[i].GetComponentInChildren<Toggle>().isOn = true;

        for (int i = rank + 1; i < rateStars.Length; i++)
            rateStars[i].GetComponentInChildren<Toggle>().isOn = false;

        currentRate = rank;
    }

    public int GetCurrentRate()
    {
        return currentRate;
    }
}