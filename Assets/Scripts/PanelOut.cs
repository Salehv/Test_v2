using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;

public class PanelOut : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		ViewManager.instance.Escape();
	}
}