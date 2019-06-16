using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionEnd : MonoBehaviour
{
    public void TransitionEnded()
    {
        TransitionHandler.instance.TransitionEnded();
    }
}