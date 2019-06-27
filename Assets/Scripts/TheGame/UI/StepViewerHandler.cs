using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepViewerHandler : MonoBehaviour
{
    public static StepViewerHandler instance;

    public Slider progressBar;
    public Text textStep1;
    public Text textStep2;
    public GameObject gem3;
    public GameObject gem2;

    private int steps1;
    private int steps2;
    private int step3Counter;
    private int stage;
    private int currentStep;

    private void Start()
    {
        instance = this;
    }

    public void Init(int steps)
    {
        gem3.GetComponent<Image>().color = Color.white;
        gem2.GetComponent<Image>().color = Color.white;

        steps-= 2;

        currentStep = 0;
        progressBar.value = 1;
        steps1 = steps;

        if (steps <= 2)
        {
            steps2 = 1;
        }
        else
        {
            steps2 = (int) steps / 3;
        }

        textStep1.text = "" + (steps1 + 1);
        textStep2.text = "" + (steps1 + steps2 +1);
    }

    public void StepForward()
    {
        if (currentStep < steps1)
        {
            progressBar.value -= (float) (0.33 / (steps1+1));
            step3Counter = 6;
            gem3.GetComponent<Image>().color = Color.white;
            gem2.GetComponent<Image>().color = Color.white;
        }
        else if (currentStep < steps1 + steps2)
        {
            progressBar.value -= (float) (0.33 / steps2);
            step3Counter = 6;
            gem3.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            gem2.GetComponent<Image>().color = Color.white;
        }
        else
        {
            step3Counter--;
            print($"step 3 Counter: {step3Counter}    --");
            if (step3Counter > 0)
            {
                progressBar.value -= 0.05f;
            }

            gem3.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            gem2.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }

        currentStep++;
        //print($"Current Steps:{currentStep}  +");
    }

    public void StepBackward()
    {
        if (currentStep <= steps1)
        {
            progressBar.value += (float) (0.33 / (steps1+1));
            step3Counter = 6;
            gem3.GetComponent<Image>().color = Color.white;
            gem2.GetComponent<Image>().color = Color.white;
        }
        else if (currentStep <= steps1 + steps2)
        {
            progressBar.value += (float) (0.33 / steps2);
            step3Counter = 6;
            gem3.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            gem2.GetComponent<Image>().color = Color.white;
        }
        else
        {
            step3Counter++;
            print($"step 3 Counter: {step3Counter}    ++");
            if (step3Counter > 1)
            {
                progressBar.value += 0.05f;
            }

            gem3.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            gem2.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }

        currentStep--;
        //print($"Current Steps:{currentStep}  -");
    }
}