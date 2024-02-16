using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffData: ScriptableObject
{
    [Header("General properties")]
    public string name;
    public string desc;
    
    [Header("Attributes to change")]
    public List<Attribute> attribute;

    [Header("Others")]
    public bool isPositive;
    public float pct;
    
}
