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
    
    public enum SkillStatus {Unavailable, Available, Learned, Equipped}

    public SkillStatus status;

    public void Learn()
    {
        if (status == SkillStatus.Available)
        {
            status = SkillStatus.Learned;
            // some more logic 
        }
    }
    
    public void Equip()
    {
        if (status == SkillStatus.Learned)
        {
            status = SkillStatus.Equipped;
            // some more logic 
        }
    }
    
    public void Unequip()
    {
        if (status == SkillStatus.Equipped)
        {
            status = SkillStatus.Learned;
            // some more logic 
        }
    }
}
