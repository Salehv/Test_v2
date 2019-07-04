using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;

public class PanelHandler : MonoBehaviour
{
    private Stack<Panel> activePanels;
    private static readonly int TRIG_PANEL_OUT = Animator.StringToHash("Panel_Out");

    private void Awake()
    {
        activePanels = new Stack<Panel>();
    }

    public void ShowPanel(Panel p)
    {
        activePanels.Push(p);
        p.gameObject.SetActive(true);
        p.Appear();
    }

    public void HideTopMostPanel()
    {
        Panel p = activePanels.Pop();
        p.Disappear();
        p.animator.SetTrigger(TRIG_PANEL_OUT);
    }

    public void HideAll()
    {
        while (isAnyPanelActive())
        {
            HideTopMostPanel();
        }
    }

    public bool isAnyPanelActive()
    {
        return activePanels.Count > 0;
    }
}