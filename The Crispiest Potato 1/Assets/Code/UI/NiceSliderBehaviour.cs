using System.Collections;
using System.Collections.Generic;
using Assets.Code.Managers;
using UnityEngine;
using UnityEngine.UI;

public class NiceSliderBehaviour : MonoBehaviour
{
    Text indicatorText;
    Text labelText;
    Slider slider;

    //public string Label = "Empty label";
    public float MinValue = 0f;
    public float DefaultValue;
    public float MaxValue = 3f;
    public float Value { get; private set; }
        
    void Start()
    {
        indicatorText = GetComponentsInChildren<Text>()[0];
        labelText = GetComponentsInChildren<Text>()[1];
        slider = GetComponentInChildren<Slider>();

        slider.maxValue = MaxValue;
        slider.minValue = MinValue;
        Value = DefaultValue;
        slider.value = Value;

        labelText.text = InitManager.HumanizeFieldName(transform.parent.name) + ": ";
        Work();
    }

    public void Work()
    {
        if (slider != null)
        {
            Value = slider.value;
            indicatorText.text = string.Format("{0:0.0}", Value);
        }
    }


}
