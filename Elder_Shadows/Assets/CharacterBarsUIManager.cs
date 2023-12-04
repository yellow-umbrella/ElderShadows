using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBarsUIManager : MonoBehaviour
{
    public static CharacterBarsUIManager instance;
    [SerializeField] private Slider healthbar;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private Slider manabar;
    [SerializeField] private Slider expbar;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        healthbar.value = 1f;
        manabar.value = 1f;
        expbar.value = 1f;
    }

    public void UpdateHealthBar(float max, float current)
    {
        healthbar.value = current / max;
        if (current <= 0)
            current = 0;
        health.text = current + "/" + max;
    }
    
    public void UpdateManaBar(float max, float current)
    {
        manabar.value = current / max;
    }
    
    public void UpdateExpBar(float max, float current)
    {
        expbar.value = current / max;
    }
}
