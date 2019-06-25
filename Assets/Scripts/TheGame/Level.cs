using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Initialize;
using UnityEngine;

public class Level
{
    internal readonly int id;
    internal DynamicsFlag flags;
    internal int gems = 0;


    internal readonly int chapterId;
    private int solvedSteps = 0;

    public readonly string begin;
    public readonly string end;
    public string[] way;
    public bool unlocked;

    public Level(int id, int chapterId, int solvedSteps, string begin, string end, int gems)
    {
        this.id = id;
        this.chapterId = chapterId;
        this.solvedSteps = solvedSteps;
        this.begin = begin;
        this.end = end;
        this.gems = gems;
    }

    public void SetDynamicFlags(DynamicsFlag flag)
    {
        flags = flag;
    }

    public void SetWay(string[] words)
    {
        way = words;
    }

    internal int CalculateCoinGain(int steps)
    {
        int best = way.Length;
        if (steps <= best)
            return 10;
        if (steps < (Math.Min(best + best * 2 / 3, best + 2)))
            return 8;

        return 5;
    }


    internal int CalculateGemGain(int steps)
    {
        int best = way.Length;
        if (steps <= best)
            return 3;
        if (steps < (Math.Min(best + (best * 2 / 3), best + 2)))
            return 2;

        return 1;
    }

    public override string ToString()
    {
        return $"[Level] Chapter:{chapterId} - ID:{id} - Begin:{begin} - End:{end}";
    }
}