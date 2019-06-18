using System.Collections;
using System.Collections.Generic;
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
        int diff = steps - way.Length;
        return 10 - (2 - diff) * 2;
    }
}