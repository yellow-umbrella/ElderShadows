using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill")]
public class SkillData : ScriptableObject
{
    public Skill skillPrefab;
    public int ID;
    [TextArea]
    public string description;
    public Sprite menuIcon;
    public enum SkillType {Active, Passive}

    public SkillType type;
}
