using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEntityVisuals : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private BaseEntity entity;

    private float maxValue;

    private void Start()
    {
        maxValue = entity.Health;
        healthBar.fillAmount = 1;
    }

    private void Update()
    {
        healthBar.fillAmount = entity.Health / maxValue;
    }
}
