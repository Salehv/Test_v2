using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QF_SliderHandler : MonoBehaviour
{
    public Text ageDesc;
    public Slider slider;
    public string[] descs;

    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = descs.Length - 1;
        slider.wholeNumbers = true;
        slider.value = 0;

        ageDesc.text = descs[0];
    }

    public void SliderChanged()
    {
        ageDesc.text = descs[(int) slider.value];
    }
}