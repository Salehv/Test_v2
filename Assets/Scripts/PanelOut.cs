using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using App;
using UnityEngine;
using UnityEngine.AI;

public class PanelOut : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if(stateInfo.IsName("FadeToBlack"))
			return;

		Debug.Log("HEHEHEHHEHEHR");
		animator.GetComponentInParent<Panel>().gameObject.SetActive(false);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		ViewManager.instance.PageBlacked();
	}
}