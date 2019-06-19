using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


public class TimeManager : MonoBehaviour
{
    internal static TimeManager instance;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }

    internal void SetRealTimer(string timer)
    {
        PlayerPrefs.SetString(timer, (DateTime.UtcNow.ToFileTimeUtc() / 10000000) + "");
        PlayerPrefs.Save();
    }

    internal int GetCurrentRealTime(string timer)
    {
        if (!PlayerPrefs.HasKey(timer))
            return -1;

        string time = PlayerPrefs.GetString(timer);
        return (int) ((DateTime.UtcNow.ToFileTimeUtc() / 10000000) - long.Parse(time));
    }


    internal void DiscardRealTimer(string timer)
    {
        if (PlayerPrefs.HasKey(name))
            return;

        PlayerPrefs.DeleteKey(timer);
        PlayerPrefs.Save();
    }

    private string toAdd = "";
    private string toRemove = "";

    internal void SetTimer(string timer)
    {
        toAdd = timer;
    }

    internal float GetCurrentTime(string timer)
    {
        return timers[timer];
    }

    internal void DiscardTimer(string timer)
    {
        toRemove = timer;
    }


    private Dictionary<string, float> timers;

    private List<string> keys;

    private void Start()
    {
        timers = new Dictionary<string, float>();
        keys = new List<string>(timers.Keys);
        ;
    }

    private void Update()
    {
        if (toAdd != "")
        {
            try
            {
                timers.Add(toAdd, 0);
                keys.Add(toAdd);
                toAdd = "";
            }
            catch (ArgumentException e)
            {
                print("[TimeManager] Exception Handled");
                timers[toAdd] = 0;
                toAdd = "";
            }
            
        }

        if (toRemove != "")
        {
            timers.Remove(toRemove);
            keys.Remove(toRemove);
            toRemove = "";
        }

        foreach (string timer in keys)
        {
            timers[timer] += Time.deltaTime;
        }
    }
}