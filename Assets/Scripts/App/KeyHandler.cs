using System;
using System.Collections;
using System.Collections.Generic;
using App;
using TheGame;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class NoKeyException : Exception
{
}

public class KeyHandler : MonoBehaviour
{
    public static KeyHandler instance;
    public bool debugMode;
    private bool initiated = false;

    private int energy;
    private int maxEnergy = 10;

    private long lastChargeTime;

    private bool isCharging;

    private int energyChargeTime = 300;

    public GameObject energyItemParent;
    public GameObject energyItemsPrefab;
    private List<GameObject> energyItems;
    public Text[] energyCountdownTimer;


    #region Init

    void Awake()
    {
        instance = this;
        energyItems = new List<GameObject>();
    }

    internal void FirstEnter()
    {
        energy = maxEnergy;
        lastChargeTime = -1;

        // TODO: Adding Max Energy    
        // PlayerPrefs.SetInt("maxEnergy", maxEnergy);

        PlayerPrefs.SetInt("energy", energy);
        PlayerPrefs.SetString("lastChargeTime", "" + lastChargeTime);
        PlayerPrefs.Save();
    }


    public void Init()
    {
        if (!PlayerPrefs.HasKey("energy") || !PlayerPrefs.HasKey("lastChargeTime"))
        {
            FirstEnter();
        }

        // TODO: Adding Max Energy
        // maxEnergy = PlayerPrefs.GetInt("maxEnergy");
        InitGraphics();

        lastChargeTime = long.Parse(PlayerPrefs.GetString("lastChargeTime"));
        if (debugMode)
            print($"[EnergyHandler] Last Charge time:{lastChargeTime}");

        UpdateEnergy();
        GraphicalUpdate();

        initiated = true;
    }

    #endregion


    private void UpdateEnergy()
    {
        if (lastChargeTime == -1)
        {
            energy = maxEnergy;
            isCharging = false;
            return;
        }

        energy = PlayerPrefs.GetInt("energy");
        if (debugMode)
            print($"[EnergyHandler] Previous Energy:{energy}");


        CalculateEnergyIncrease();
    }

    private void CalculateEnergyIncrease()
    {
        long absentTime = Now() - lastChargeTime;

        if (debugMode)
            print($"[EnergyHandler] Absent Time:{absentTime}");

        while (absentTime > energyChargeTime)
        {
            IncreaseEnergy();
            absentTime -= energyChargeTime;
        }

        if (debugMode)
            print($"[EnergyHandler] Current Energy:{energy}");

        lastChargeTime = Now() - absentTime;
        PlayerPrefs.SetString("lastChargeTime", "" + lastChargeTime);
        PlayerPrefs.Save();

        isCharging = true;
        if (energy == maxEnergy)
        {
            lastChargeTime = -1;
            isCharging = false;
            PlayerPrefs.SetString("lastChargeTime", "-1");
            PlayerPrefs.Save();
        }
    }

    private void IncreaseEnergy(bool purchase = false)
    {
        energy += 1;

        PlayerPrefs.SetInt("energy", energy);
        PlayerPrefs.Save();

        if (!purchase)
            lastChargeTime += energyChargeTime;
        PlayerPrefs.SetString("lastChargeTime", "" + lastChargeTime);
        PlayerPrefs.Save();

        if (energy >= maxEnergy)
        {
            energy = maxEnergy;
            lastChargeTime = -1;
            PlayerPrefs.SetString("lastChargeTime", "-1");
            PlayerPrefs.Save();

            isCharging = false;
        }

        GraphicalUpdate();
    }


    public void UseEnergy()
    {
        if (energy > 0)
        {
            energy -= 1;
            PlayerPrefs.SetInt("energy", energy);
            PlayerPrefs.Save();

            if (lastChargeTime == -1)
            {
                lastChargeTime = Now();
                isCharging = true;
                PlayerPrefs.SetString("lastChargeTime", lastChargeTime + "");
                PlayerPrefs.Save();
            }

            if (debugMode)
                print($"[EnergyHandler] Energy Used!. Now you have {energy} energies.");

            GraphicalUpdate();
        }
        else
        {
            throw new NoKeyException();
        }
    }


    private long Now()
    {
        return DateTime.UtcNow.ToFileTimeUtc() / 10000000;
    }

    #region Graphics

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
            energyItems[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180 / maxEnergy * i - 45);
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

    #endregion


    private void EnergyCountDownTimerUpdate()
    {
        if (isCharging)
            foreach (var t in energyCountdownTimer)
            {
                t.text = Utilities.GetTimeFormat(energyChargeTime - (Now() - lastChargeTime) + 1);
            }
        else
            foreach (var t in energyCountdownTimer)
            {
                t.text = "";
            }
    }

    private void Update()
    {
        if (initiated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                UseEnergy();


            EnergyCountDownTimerUpdate();

            if (Now() - lastChargeTime > energyChargeTime && isCharging)
                IncreaseEnergy();
        }
    }

    internal void AddKeys(int keys)
    {
        for (int i = 0; i < keys; i++)
            IncreaseEnergy(true);
    }

    public void HalfKey()
    {
        try
        {
            GameManager.instance.AddCoins(-60);
        }
        catch (NoMoneyException e)
        {
            PopupHandler.ShowNoMoney(60 - GameManager.instance.GetCoins());
            return;
        }


        try
        {
            AddKeys(5);
        }
        catch (Exception e)
        {
        }

        ViewManager.instance.Escape();
        ViewManager.instance.Escape();
    }

    public void FullKey()
    {
        try
        {
            GameManager.instance.AddCoins(-100);
        }
        catch (NoMoneyException e)
        {
            PopupHandler.ShowNoMoney(100 - GameManager.instance.GetCoins());
            return;
        }

        try
        {
            AddKeys(10);
        }
        catch (Exception e)
        {
        }

        ViewManager.instance.Escape();
        ViewManager.instance.Escape();
    }
}