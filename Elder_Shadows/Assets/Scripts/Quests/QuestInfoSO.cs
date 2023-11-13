using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class QuestInfoSO : ScriptableObject
{
    [Foldout("General", true)]
    [TextArea(1, 5)]
    public string displayName;
    [TextArea(5, 20)] 
    public string description;
    [TextArea(5, 20)] 
    public string objectives;

    [Foldout("Requirements", true)]
    public int levelRequirement;
    public int trustRequirement;

    [Foldout("Rewards", true)]
    public int expirients;
    public int trust;
    public int money;
    public GameObject[] items;
}
