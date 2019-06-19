using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class EnergyHandler : MonoBehaviour
{
    public static EnergyHandler instance;
    
    private int energy;
    private int maxEnergy = 10;
    private bool isLastLevelUnlocked;
    private long lastUsedEnergy;
    private int energyChargeTime = 30;

    public GameObject energyItemParent;
    public GameObject energyItemsPrefab;
    private List<GameObject> energyItems;
    public Text[] energyCountdownTimer;

    public void Init()
    {
        if (!PlayerPrefs.HasKey("energy"))
        {
            energy = maxEnergy;
            isLastLevelUnlocked = true;
            lastUsedEnergy = -1;
            
            
            PlayerPrefs.SetInt("maxEnergy", maxEnergy);
            PlayerPrefs.SetInt("energy", energy);
            PlayerPrefs.SetInt("isLastLevelUnlocked", isLastLevelUnlocked ? 1 : 0);
            PlayerPrefs.SetString("lastUsedEnergy", "" + lastUsedEnergy);
            PlayerPrefs.Save();
        }

        maxEnergy = PlayerPrefs.GetInt("maxEnergy");
        lastUsedEnergy = long.Parse(PlayerPrefs.GetString("lastUsedEnergy"));
        
        UpdateEnergy();
        //energy = 3; //TODO: erase this shit
        GraphicalUpdate();
    }
    public void UpdateEnergy()
    {
        if (lastUsedEnergy == -1)
        {
            energy = maxEnergy;
            return;
        }
        
        energy = PlayerPrefs.GetInt("energy");
        
        CalculateEnergyIncrease();
    }
    public void CalculateEnergyIncrease()
    {
        long now = DateTime.UtcNow.ToFileTimeUtc() / 10000000;
        long absentTime = now - lastUsedEnergy;
        while (absentTime > energyChargeTime && energy < maxEnergy)
        {
            energy += 1;
            lastUsedEnergy += energyChargeTime;
            absentTime = now - lastUsedEnergy;
        }

        if (energy == maxEnergy)
        {
            lastUsedEnergy = -1;
        }
    }
    public void UseEnergy()
    {
        if (energy != 0)
        {
            energy -= 1;
            if (lastUsedEnergy == -1)
            {
                lastUsedEnergy = DateTime.UtcNow.ToFileTimeUtc() / 10000000;
            }
        }
        else
        {
            throw new NoEnergyException();
        }
    }

    public void GraphicalUpdate()
    {
        for (int i = 0; i < energyItemParent.transform.childCount; i++)
        {
            Destroy(energyItemParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < energy; i++)
        {
            energyItems.Add(Instantiate(energyItemsPrefab, energyItemParent.transform));
            energyItems[i].GetComponent<RectTransform>().anchoredPosition = energyItems[i].GetComponent<RectTransform>()
                .anchoredPosition.Rotate(180 / maxEnergy * i - 45);
            energyItems[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0, 180 / maxEnergy * i - 45);
        }
        for (int i = energy; i < maxEnergy; i++)
        {
            energyItems.Add(Instantiate(energyItemsPrefab, energyItemParent.transform));
            energyItems[i].GetComponent<RectTransform>().anchoredPosition = energyItems[i].GetComponent<RectTransform>()
                .anchoredPosition.Rotate(180 / maxEnergy * i - 45);
            energyItems[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0, 180 / maxEnergy * i - 45);
            energyItems[i].GetComponent<Image>().color = new Color(1,1,1,0.3f); 
        }
    }
    void Awake()
    {
        instance = this;
        energyItems = new List<GameObject>();
        Init();
    }

    public void EnergyCountDownTimerUpdate()
    {
        for (int i = 0; i < energyCountdownTimer.Length; i++)
        {
            energyCountdownTimer[i].text ="" + (lastUsedEnergy - DateTime.UtcNow.ToFileTimeUtc() / 10000000);
        }
    }
    void Update()
    {
    }
}

public class NoEnergyException : Exception
{
    
}