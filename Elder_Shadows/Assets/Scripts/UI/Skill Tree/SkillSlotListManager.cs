using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class SkillSlotListManager : MonoBehaviour
{
    [SerializeField] private SkillsDataBase skillsDataBase;
    [SerializeField] private GameObject InteractButton;
    [SerializeField] private GameObject[] SkillSlots;
    private SkillData ChosenSkillData;

    private void OnEnable()
    {
        ChosenSkillData = InteractButton.GetComponent<InteractButtonManager>().skillNode.GetComponent<SkillNode>().skillData;
        for (int i = 0; i < SkillSlots.Length; i++)
        {
            SkillSlots[i].GetComponent<ChooseSkillSlotManager>().ChosenSkill = skillsDataBase.EquippedSkills[i];
            SkillSlots[i].GetComponent<ChooseSkillSlotManager>().UpdateHeader();
            if (SkillSlots[i].GetComponent<ChooseSkillSlotManager>().index == -1)
                SkillSlots[i].GetComponent<ChooseSkillSlotManager>().index = i;
        }
    }

    public void ChooseSlot(GameObject slot)
    {
        slot.GetComponent<ChooseSkillSlotManager>().ChosenSkill = ChosenSkillData;
        if (skillsDataBase.EquippedSkills[slot.GetComponent<ChooseSkillSlotManager>().index] != null)
            skillsDataBase.EquippedSkills[slot.GetComponent<ChooseSkillSlotManager>().index].Unequip();
        skillsDataBase.EquippedSkills[slot.GetComponent<ChooseSkillSlotManager>().index] = ChosenSkillData;
        ChosenSkillData.Equip();
    }

    public void SetInactive()
    {
        gameObject.SetActive(false);
        ChosenSkillData.Unequip();
    }
}
