using System;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UPersian.Utils;

public class QuestionFormHandler : MonoBehaviour
{
    public GameObject age;
    public GameObject favGames;
    public GameObject playTime;
    public GameObject difficulty;
    public GameObject missing;
    private PlayerData playerData;

    public void Check(bool isIntended)
    {
        playerData = new PlayerData();

        Toggle[] ts;

        int ageCode = 0;
        ts = age.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < age.transform.childCount; i++)
        {
            if (age.transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                ageCode += (int) Math.Pow(2, i);
            }
        }

        int favGameCode = 0;
        ts = favGames.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < favGames.transform.childCount; i++)
        {
            if (favGames.transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                favGameCode += (int) Math.Pow(2, i);
            }
        }

        int playTimeCode = 0;
        ts = playTime.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < playTime.transform.childCount; i++)
        {
            if (playTime.transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                playTimeCode += (int) Math.Pow(2, i);
            }
        }

        int difficultyCode = 0;
        ts = difficulty.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < difficulty.transform.childCount; i++)
        {
            if (difficulty.transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                difficultyCode += (int) Math.Pow(2, i);
            }
        }

        int missingCode = 0;
        ts = missing.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < missing.transform.childCount; i++)
        {
            if (missing.transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                missingCode += (int) Math.Pow(2, i);
            }
        }

        playerData.age = ageCode;
        playerData.favGames = favGameCode;
        playerData.playTime = playTimeCode;
        playerData.difficulty = difficultyCode;
        playerData.missing = missingCode;


        StartCoroutine(SendInformation(isIntended));
    }

    private void EmptyFieldError(string field)
    {
        PopupHandler.ShowDebug("لطفا قسمت " + field + " را تکمیل کنید");
        print("error " + field);
    }

    private IEnumerator SendInformation(bool isIntended)
    {
        string json = JsonUtility.ToJson(playerData);
        try
        {
            using (WebClient client = new WebClient())
            {
                string s = client.UploadString("http://support.qolenj.ir/kalanjar/get_stats.php?data=" + json, "");
                Debug.Log(s);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Nashod ke beshe!");
        }

        if (isIntended)
            PopupHandler.ShowInfo("ازینکه پرسشنامه رو پر کردین متشکریم :)", reward);

        yield return null;
    }

    private void reward()
    {
        ChestHandler.instance.CallChest(50, 0);
    }
}