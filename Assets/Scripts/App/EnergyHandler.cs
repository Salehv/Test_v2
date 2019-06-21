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
    public bool debugMode;    
    
    private int energy;
    private int maxEnergy = 10;
    
    private long lastUsedEnergy;
    private long lastChargeTime;

    private int energyChargeTime = 30;

    public GameObject energyItemParent;
    public GameObject energyItemsPrefab;
    private List<GameObject> energyItems;
    public Text[] energyCountdownTimer;

    internal void FirstEnter()
    {
        energy = maxEnergy;
        lastUsedEnergy = -1;
            
        // TODO: Adding Max Energy    
        // PlayerPrefs.SetInt("maxEnergy", maxEnergy);
        PlayerPrefs.SetInt("energy", energy);
        PlayerPrefs.SetString("lastUsedEnergy", "" + lastUsedEnergy);
        PlayerPrefs.Save();
    }
    
    public void Init()
    {
        if (!PlayerPrefs.HasKey("energy") || !PlayerPrefs.HasKey("lastUsedEnergy"))
        {
            FirstEnter();
        }

        // TODO: Adding Max Energy
        // maxEnergy = PlayerPrefs.GetInt("maxEnergy");
        InitGraphics();
        
        
        lastUsedEnergy = long.Parse(PlayerPrefs.GetString("lastUsedEnergy"));
        if(debugMode)
            print($"[EnergyHandler] Last use of energy:{lastUsedEnergy}");
        
        UpdateEnergy();
        GraphicalUpdate();
    }

    private void UpdateEnergy()
    {
        if(lastUsedEnergy == -1)
        {
            energy = maxEnergy;
            return;
        }
        
        energy = PlayerPrefs.GetInt("energy");
        if(debugMode)
            print($"[EnergyHandler] Previous Energy:{energy}");

        
        CalculateEnergyIncrease();
    }

    private void CalculateEnergyIncrease()
    {
        long absentTime = Now() - lastUsedEnergy;
        
        if(debugMode)
            print($"[EnergyHandler] Absent Time:{absentTime}");
        
        while (absentTime > energyChargeTime)
        {
            IncreaseEnergy();
            absentTime -= energyChargeTime;
        }
        
        if(debugMode)
            print($"[EnergyHandler] Current Energy:{energy}");


        if (energy == maxEnergy)
        {
            lastUsedEnergy = -1;
            PlayerPrefs.SetString("lastUsedEnergy", "-1");
            PlayerPrefs.Save();
        }
    }

    private long Now()
    {
        return DateTime.UtcNow.ToFileTimeUtc() / 10000000;
    }

    private void IncreaseEnergy()
    {
        energy += 1;
        lastChargeTime = Now();    
        
        if (energy > maxEnergy)
            energy = maxEnergy;
    }


    public void UseEnergy()
    {
        if (energy > 0)
        {
            energy -= 1;
            PlayerPrefs.SetInt("energy", energy);
            PlayerPrefs.Save();
            
            if (lastUsedEnergy == -1)
            {
                lastUsedEnergy = Now();
                PlayerPrefs.SetString("lastUsedEnergy", lastUsedEnergy + "");
                PlayerPrefs.Save();
                
            }
            
            if(debugMode)
                print($"[EnergyHandler] Energy Used!. Now you have {energy} energies.");
            
            GraphicalUpdate();
        }
        else
        {
            throw new NoEnergyException();
        }
    }

    private void GraphicalUpdate()
    {
        for (int i = 0; i < energy; i++)
        {
            energyItems[i].GetComponent<Image>().color = Color.white;
        }

        for (int i = energy; i < maxEnergy; i++)
        {
            energyItems[i].GetComponent<Image>().color = Color.gray;
        }
    }

    private void InitGraphics()
    {
        for (int i = 0; i < energyItemParent.transform.childCount; i++)
        {
            Destroy(energyItemParent.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < maxEnergy; i++)
        {
            energyItems.Add(Instantiate(energyItemsPrefab, energyItemParent.transform));
            energyItems[i].GetComponent<RectTransform>().anchoredPosition = energyItems[i].GetComponent<RectTransform>()
                .anchoredPosition.Rotate(180 / maxEnergy * i - 45);
            energyItems[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0, 180 / maxEnergy * i - 45);
        }
    }
    void Awake()
    {
        instance = this;
        energyItems = new List<GameObject>();
    }

    public void EnergyCountDownTimerUpdate()
    {
        for (int i = 0; i < energyCountdownTimer.Length; i++)
        {
            energyCountdownTimer[i].text ="" + (lastUsedEnergy - DateTime.UtcNow.ToFileTimeUtc() / 10000000);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            UseEnergy();
    }
}

public class NoEnergyException : Exception{}