using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class EnergyHandler : MonoBehaviour
{
    public static EnergyHandler instance;
    
    private int energy;
    private int maxEnergy;
    private bool isLastLevelUnlocked;
    private long lastUsedEnergy;
    private bool isCharging;
    private int energyChargeTime;


    public void FirstInit()
    {
        maxEnergy = 10;
        energy = maxEnergy;
        isLastLevelUnlocked = true;
        energyChargeTime = 3600;
        
        PlayerPrefs.SetInt("maxEnergy", maxEnergy);
        PlayerPrefs.SetInt("energy", energy);
        PlayerPrefs.Save();
    }
    public void UseEnergy()
    {
        if (energy == 0)
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


    public int EnergyLeft()
    {
        return energy;
    }

    public void UpdateEnergy()
    {
        long now = DateTime.UtcNow.ToFileTimeUtc() / 10000000;
        while (now - lastUsedEnergy > energyChargeTime && energy < maxEnergy)
        {
            energy += 1;
            lastUsedEnergy += energyChargeTime;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UpdateEnergy();
    }
}

public class NoEnergyException : Exception
{
    
}