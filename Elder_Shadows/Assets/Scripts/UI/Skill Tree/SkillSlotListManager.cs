using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class SkillSlotListManager : MonoBehaviour
{
    [SerializeField] private SkillsDataBase skillsDataBase;
    [SerializeField] private GameObject InteractButton;
    [SerializeField] private SkillTreeManager skillTreeManager;
    [SerializeField] private GameObject[] SkillSlots;
    private SkillData ChosenSkillData;

    private void OnEnable()
    {
        ChosenSkillData = InteractButton.GetComponent<InteractButtonManager>().skillNode.GetComponent<SkillNode>().skillData;
        for (int i = 0; i < SkillSlots.Length; i++)
        {
            var tmp = SkillSlots[i].GetComponent<ChooseSkillSlotManager>();
            tmp.ChosenSkill = skillsDataBase.EquippedSkills[i];
            tmp.UpdateHeader();
            if (tmp.index == -1)
                tmp.index = i;
        }
    }

    public void ChooseSlot(GameObject slot)
    {
        var tmp = slot.GetComponent<ChooseSkillSlotManager>();
        tmp.ChosenSkill = ChosenSkillData;
        if (skillsDataBase.EquippedSkills[tmp.index] != null)
        {
            skillsDataBase.EquippedSkills[tmp.index].Unequip();
            skillTreeManager.skillNodes.Find(n => n.GetComponent<SkillNode>().skillData == skillsDataBase.EquippedSkills[tmp.index])
                .GetComponent<SkillNode>().RefreshNode();
        }
        skillsDataBase.EquippedSkills[tmp.index] = ChosenSkillData;
        ChosenSkillData.Equip();
        InteractButton.GetComponent<InteractButtonManager>().skillNode.GetComponent<SkillNode>().RefreshNode();
    }

    public void SetInactive()
    {
        gameObject.SetActive(false);
        ChosenSkillData.Unequip();
    }
}
