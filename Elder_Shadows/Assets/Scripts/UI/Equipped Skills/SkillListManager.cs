using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillListManager : MonoBehaviour
{
    [SerializeField] private SkillsDataBase skillsDataBase;
    [SerializeField] private GameObject SkillHeaderPrefab;
    [SerializeField] private GameObject[] ActiveSkills = new GameObject[3];
    [SerializeField] private GameObject PassiveSkillsObject;
    [SerializeField] private GameObject skillTreeTabOutline;
    [SerializeField] private GameObject skillTreePage;
    [SerializeField] private GameObject equippedSkillsTabOutline;
    [SerializeField] private GameObject equippedSkillsPage;
    [SerializeField] private GameObject emptyDescriptionObj;
    [SerializeField] private GameObject descriptionObj;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Start()
    {
        RefreshLists();
    }

    private void OnEnable()
    {
        RefreshLists();
    }

    void RefreshLists()
    {
        for (int i = 0; i < skillsDataBase.GetCount(); i++)
        {
            var tmpSkill = skillsDataBase.GetSkill(i); 
            if (tmpSkill.type == SkillData.SkillType.Passive && tmpSkill.status == SkillData.SkillStatus.Learned && 
                !PassiveSkillsObject.GetComponent<PassiveSkillsHeaderManager>().PassiveSkillsList.Contains(tmpSkill))
            {
                GameObject skillHeader = Instantiate(SkillHeaderPrefab, Vector3.zero, Quaternion.identity, transform);
                skillHeader.transform.SetParent(PassiveSkillsObject.transform, false);

                skillHeader.GetComponent<SkillHeaderManager>().ChosenSkill = tmpSkill;
                skillHeader.GetComponent<SkillHeaderManager>().SkillListManager = gameObject;
                skillHeader.GetComponent<SkillHeaderManager>().UpdateHeader();
                
                PassiveSkillsObject.GetComponent<PassiveSkillsHeaderManager>().PassiveSkillsList.Add(tmpSkill);
            }
        }

        for (int i = 0; i < skillsDataBase.EquippedSkills.Length; i++)
        {
            if (skillsDataBase.EquippedSkills[i] == null)
            {
                SetHeaderDefault(ActiveSkills[i]);
            }
            else
            {
                var skillHeader = ActiveSkills[i].GetComponent<SkillHeaderManager>();
                if (skillHeader.ChosenSkill == null || skillHeader.ChosenSkill.ID != skillsDataBase.EquippedSkills[i].ID)
                {
                    skillHeader.ChosenSkill = skillsDataBase.EquippedSkills[i];
                    skillHeader.SkillListManager = gameObject;
                }

                if (skillHeader.index == -1)
                    skillHeader.index = i;

                if (skillsDataBase.EquippedSkills[i].status != SkillData.SkillStatus.Equipped)
                    skillsDataBase.EquippedSkills[i] = null;
                
                skillHeader.UpdateHeader();
            }
        }
    }

    public void ChooseHeader(GameObject header)
    {
        if (!descriptionObj.activeSelf)
        {
            emptyDescriptionObj.SetActive(false);
            descriptionObj.SetActive(true);
        }

        var headerSkillData = header.GetComponent<SkillHeaderManager>().ChosenSkill;
        if (headerSkillData != null)
        {
            headerText.text = headerSkillData.name;
            descriptionText.text = headerSkillData.description;
        }
    }

    public void SetHeaderDefault(GameObject header)
    {
        if (!descriptionObj.activeSelf)
        {
            emptyDescriptionObj.SetActive(true);
            descriptionObj.SetActive(false);
        }

        header.GetComponent<SkillHeaderManager>().ChosenSkill = null;
        headerText.text = "";
        descriptionText.text = "";
        header.GetComponent<SkillHeaderManager>().UpdateHeader();
    }

    public void OpenSkillTree()
    {
        equippedSkillsPage.SetActive(false);
        skillTreePage.SetActive(true);
        
        equippedSkillsTabOutline.SetActive(false);
        skillTreeTabOutline.SetActive(true);
    }

    public void UnequipSkill(int i)
    {
        skillsDataBase.EquippedSkills[i].Unequip();
        skillsDataBase.EquippedSkills[i] = null;
    }
}

public static class SkillListData
{
    public static GameObject PreviousHeader;
}