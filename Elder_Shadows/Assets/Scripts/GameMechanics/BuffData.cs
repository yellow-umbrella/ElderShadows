using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffData: ScriptableObject
{
    [Header("General properties")]
    public string name;
    public string desc;

    [Header("Attributes to change")]
    public Attributes attribute;

    [Header("Others")]
    public bool isPositive;

    public enum BuffType
    {
        AttributeMod,
        HealthOverTime,
        ManaMod
    }
    public bool isPercentile;
    public BuffType type;
    public IAttackable.DamageType dmgType;
    public float value;
    public float tickrate;

}
