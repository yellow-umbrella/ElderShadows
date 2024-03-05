using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills database")]
public class SkillsDataBase : ScriptableObject
{
    [SerializeField]
    private List<SkillData> skills;

    public SkillData GetSkill(int skillID)
    {
        return skills[skillID];
    }
    
    public int GetCount()
    {
        return skills.Count;
    }
}
