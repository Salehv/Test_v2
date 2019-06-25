using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    static Utilities()
    {
        InitDictionaries();
    }

    internal static string GetOnlyFarsi(string str)
    {
        string result = "";

        for (int i = 0; i < str.Length; i++)
        {
            if ("ابپتثجچحخدذرزژسشصضطظعغفقکگلمنوهی".Contains(str[i] + ""))
                result += str[i];
        }

        return result;
    }

    internal static string GetTimeFormat(float time)
    {
        return $"{((int) time) / 60:00}:{(int) time % 60:00}";
    }

    internal static string GetCompletionText(int current, int max)
    {
        return string.Format("{0:00}/{1:00}", current, max);
    }


    internal static Dictionary<char, int> dic_charToLetter;
    internal static Dictionary<int, char> dic_letterToChar;

    private static void InitDictionaries()
    {
        dic_charToLetter = new Dictionary<char, int>
        {
            {' ', -1},
            {'ا', 0},
            {'آ', 0},
            {'ب', 1},
            {'پ', 2},
            {'ت', 3},
            {'ث', 4},
            {'ج', 5},
            {'چ', 6},
            {'ح', 7},
            {'خ', 8},
            {'د', 9},
            {'ذ', 10},
            {'ر', 11},
            {'ز', 12},
            {'ژ', 13},
            {'س', 14},
            {'ش', 15},
            {'ص', 16},
            {'ض', 17},
            {'ط', 18},
            {'ظ', 19},
            {'ع', 20},
            {'غ', 21},
            {'ف', 22},
            {'ق', 23},
            {'\u06A9', 24},
            {'\uFB8E', 24},
            {'\uFB8F', 24},
            {'\uFB90', 24},
            {'\uFB91', 24},
            {'گ', 25},
            {'ل', 26},
            {'م', 27},
            {'ن', 28},
            {'و', 29},
            {'ه', 30},
            {'\u064A', 31},
            {'\u0649', 31},
            {'\uFEF1', 31},
            {'\uFEF2', 31},
            {'\uFEF3', 31},
            {'\uFEF4', 31},
            {'\uFEEF', 31},
            {'\uFEF0', 31},
            {'\u06CC', 31}
        };


        dic_letterToChar = new Dictionary<int, char>
        {
            {-1, ' '},
            {0, 'ا'},
            {1, 'ب'},
            {2, 'پ'},
            {3, 'ت'},
            {4, 'ث'},
            {5, 'ج'},
            {6, 'چ'},
            {7, 'ح'},
            {8, 'خ'},
            {9, 'د'},
            {10, 'ذ'},
            {11, 'ر'},
            {12, 'ز'},
            {13, 'ژ'},
            {14, 'س'},
            {15, 'ش'},
            {16, 'ص'},
            {17, 'ض'},
            {18, 'ط'},
            {19, 'ظ'},
            {20, 'ع'},
            {21, 'غ'},
            {22, 'ف'},
            {23, 'ق'},
            {24, 'ک'},
            {25, 'گ'},
            {26, 'ل'},
            {27, 'م'},
            {28, 'ن'},
            {29, 'و'},
            {30, 'ه'},
            {31, '\u06CC'}
        };
    }

    internal static string GetNormalizedFarsi(string simpleFarsi)
    {
        string result = "";

        for (int i = 0; i < simpleFarsi.Length; i++)
        {
            try
            {
                if (simpleFarsi[i] == '\n')
                {
                    result += "\n";
                    continue;
                }

                result = result + dic_letterToChar[dic_charToLetter[simpleFarsi[i]]];
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError(string.Format("[Utilities] Letter is not recognized in <{0}> until <{1}>", simpleFarsi,
                    result));
                result += simpleFarsi[i];
            }
        }


        return result;
    }

    internal static int GetLevensteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        // Step 1
        if (n == 0)
            return m;
        if (m == 0)
            return n;

        // Step 2
        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }

        // Step 3
        for (int i = 1; i <= n; i++)
        {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
                // Step 5
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                // Step 6
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        // Step 7
        return d[n, m];
    }


    internal static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}