using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPersian.Components;

public class CharacterCommentHandler : MonoBehaviour
{
	internal static CharacterCommentHandler instance;
	
	public GameObject character;
	public GameObject comment;


	private void Awake()
	{
		instance = this;
	}

	public void Call(string cm , bool withCharacter = true)
	{
		character.SetActive(true);
		if (withCharacter)
		{
			comment.SetActive(true);
		}
		else
		{
			comment.SetActive(false);
		}
		comment.GetComponentInChildren<RtlText>().text = cm;
		
	}

	public void DestroyCharacter()
	{
		character.SetActive(false);
		comment.SetActive(false);
	}
}
