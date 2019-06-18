using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;

public class PanelOut : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if(stateInfo.IsName("FadeToBlack"))
			return;
		
		ViewManager.instance.Escape();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		ViewManager.instance.PageBlacked();
	}
}