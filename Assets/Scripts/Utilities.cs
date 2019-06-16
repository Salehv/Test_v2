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
}
