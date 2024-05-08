using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider slider;

    public void SetValue(int value, int maxValue) 
    {
        slider.maxValue = maxValue;
        slider.value = value;
    }
}
