using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeControllerBehaviour : MonoBehaviour
{
    Text timescaleText;
    Slider slider;

    public float MaxTimescale = 3f;
    public float Value { get; private set; }

    void Start()
    {
        Time.timeScale = 1;
        timescaleText = GetComponentInChildren<Text>();
        slider = GetComponentInChildren<Slider>();
    }

    void Update()
    {

    }

    public void Work()
    {
        if (slider != null)
        {
            Value = slider.value * MaxTimescale;
            timescaleText.text = string.Format("{0:0.0}", Value);
            GameManager.TimeScale = Value;
        }
    }


}
