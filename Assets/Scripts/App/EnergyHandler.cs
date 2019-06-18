using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class EnergyHandler : MonoBehaviour
{
    public static EnergyHandler instance;
    
    private int energy;
    private int maxEnergy;
    private bool isLastLevelUnlocked;
    private long lastUsedEnergy;
    private bool isCharging;
    private int energyChargeTime;

    public GameObject energyItemParent;
    public GameObject energyItemsPrefab;
    private List<GameObject> energyItems;


    public void FirstInit()
    {
        maxEnergy = 10;
        energy = maxEnergy;
        
        
        
        
        energy = 3; //TODO: erase this shit
        
        
        
        
        isLastLevelUnlocked = true;
        energyChargeTime = 3600;
        lastUsedEnergy = DateTime.UtcNow.ToFileTimeUtc() / 10000000;
        
        PlayerPrefs.SetInt("maxEnergy", maxEnergy);
        PlayerPrefs.SetInt("energy", energy);
        PlayerPrefs.Save();
    }
    public void UseEnergy()
    {
        if (energy != 0)
        {
            energy -= 1;
            if (!isCharging)
            {
                lastUsedEnergy = DateTime.UtcNow.ToFileTimeUtc() / 10000000;
                isCharging = true;
            }
        }
        else
        {
            throw new NoEnergyException();
        }
    }

    public void IncreaseEnergy()
    {
        energy += 1;
        if (energy == maxEnergy)
        {
            isCharging = false;
            lastUsedEnergy = -1;
        }
        else
        {
            lastUsedEnergy = DateTime.UtcNow.ToFileTimeUtc() / 10000000;
        }
    }

    public void UpdateEnergy()
    {
        long now = DateTime.UtcNow.ToFileTimeUtc() / 10000000;
        while (now - lastUsedEnergy > energyChargeTime && energy < maxEnergy)
        {
            energy += 1;
            lastUsedEnergy += energyChargeTime;
        }
        
        GraphicalUpdate();
    }

    public void GraphicalUpdate()
    {
        for (int i = 0; i < energyItemParent.transform.childCount; i++)
        {
            Destroy(energyItemParent.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < maxEnergy-energy; i++)
        {
            energyItems.Add(Instantiate(energyItemsPrefab, energyItemParent.transform));
            energyItems[i].GetComponent<Image>().color = new Color(1,1,1,0.3f); 
        }
        
        for (int i = 0; i < energy; i++)
        {
            energyItems.Add(Instantiate(energyItemsPrefab, energyItemParent.transform));
        }
    }

    void Awake()
    {
        instance = this;
        energyItems = new List<GameObject>();
        
        
        
        FirstInit();//TODO: and also erase this shit
        
        
        
        
        
        
        GraphicalUpdate();
    }

    void Update()
    {
    }
}

public class NoEnergyException : Exception
{
    
}