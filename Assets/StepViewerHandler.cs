using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class StepViewerHandler : MonoBehaviour
{
    public static StepViewerHandler instance;
    
    public Text textStep1;
    public Text textStep2;
    public Slider progressBar;

    private int steps1;
    private int steps2;
    private int stage;
    private int currentStep;

    private void Start()
    {
        instance = this;
    }

    public void Init(int steps)
    {
        currentStep = 0;
        progressBar.value = 1;
        steps1 = steps;
        if (steps < 2)
        {
            steps2 = 1;
        }
        else
        {
            steps2 = (int) steps / 3;
        }

        textStep1.text = ""+ steps1;
        textStep2.text = ""+ (steps1 + steps2);
    }

    public void StepForward()
    {
        currentStep++;
        //print($"Current Steps:{currentStep}  +");
        
        if (currentStep < steps1)
        {
            //print($"currentStep < steps1 {(0.33 / steps1)}");
            progressBar.value -= (float)(0.33 / steps1);
        }
        else if (currentStep < steps1 + steps2)
        {
            //print($"currentStep < steps1 + steps2 {(0.33 / steps2)}");
            progressBar.value -= (float)(0.33 / steps2);
        }
        else
        {
            progressBar.value -= (float) (progressBar.value / 4);
        }
    }

    public void StepBackward()
    {
        currentStep--;
        //print($"Current Steps:{currentStep}  -");
        
        if (currentStep <= steps1)
        {
            //print($"currentStep <= steps1 {(0.33 / steps1)}");
            progressBar.value += (float)(0.33 / steps1);
        }
        else if (currentStep <= steps1 + steps2)
        {
            //print($"currentStep <= steps1 + steps2 {(0.33 / steps2)}");
            progressBar.value += (float)(0.33 / steps2);
        }
        else
        {
            progressBar.value += (float) (progressBar.value / 4);
        }
    }
}
