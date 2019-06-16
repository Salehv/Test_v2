using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintWordAppeared : MonoBehaviour
{


	public static bool lastWord = false;
	
	public void NextWord()
	{
		GameManager.instance.NextWord();
		
	}

	public void DestroyWord()
	{
		Destroy(gameObject);
		if (lastWord)
		{
			GameManager.instance.EndShowWayHint();
			lastWord = false;
		}
	}


	public void DestroyShine()
	{
		Destroy(transform.parent.gameObject);
	}

	public void StartShowingHint()
	{
		GameManager.instance.Hint_ShowWayStartAnimation();
	}
}
