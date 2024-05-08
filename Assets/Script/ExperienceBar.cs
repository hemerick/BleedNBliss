using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public Slider expSlider;
    private static ExperienceBar instance;

    public static ExperienceBar GetInstance() => instance;

    public int maxExpValue = 32;

    private void Awake()
    {
        instance = this;
    }

    public void SetExperience(int experience) 
    {
        expSlider.maxValue = maxExpValue;
        expSlider.value = experience;
    }
}
