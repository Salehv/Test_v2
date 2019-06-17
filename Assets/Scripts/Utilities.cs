public static class Utilities
{
    internal static string FarsiNormalize(string str)
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
        return (((int) time) / 60 + ":" + ((int) time % 60));
    }
    
    internal static string GetCompletionText(int current, int max)
    {
        return string.Format("{0:00}/{1:00}", current, max);
    }
}