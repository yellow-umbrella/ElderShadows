using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modern2D;

public class DaycycleManager : MonoBehaviour
{
    public LightingSystem light;
    [SerializeField]private float daycycleDuration = 600f; //in seconds
    [SerializeField] private float maxShadowLength;
    [SerializeField] private Color nightLightColor;
    [SerializeField] private Color dayLightColor;
    [SerializeField] private Color sunsetLightColor;
    [SerializeField] private Color sunriseLightColor;
    
    private float currentTime = 0f;

    private void Start()
    {
        light.directionalLightAngle.value = 80f;
        light._shadowLength.value = 2f;
    }

    void Update()
    {
        light.directionalLightAngle.value -= (160f / daycycleDuration * Time.deltaTime);
        if (currentTime < daycycleDuration * 0.25f)
        {
            light._shadowLength.value -= 1.75f / daycycleDuration * Time.deltaTime;
        }
        else if (currentTime < daycycleDuration * 0.5f)
        {
            light._shadowLength.value -= 1.75f / daycycleDuration * Time.deltaTime;
        }
        else if (currentTime < daycycleDuration * 0.75f)
        {
            light._shadowLength.value += 1.75f / daycycleDuration * Time.deltaTime;
        }
        else if (currentTime < daycycleDuration)
        {
            light._shadowLength.value += 1.75f / daycycleDuration * Time.deltaTime;
        }
        else if (currentTime >= daycycleDuration)
        {
            currentTime = 0f;
            light._shadowLength.value = 2f;
            light.directionalLightAngle.value = 80f;
        }

        currentTime += Time.deltaTime;
    }
}
