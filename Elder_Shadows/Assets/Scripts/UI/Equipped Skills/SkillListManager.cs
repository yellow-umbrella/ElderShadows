using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillListManager : MonoBehaviour
{
    [SerializeField] private SkillsDataBase skillsDataBase;
    [SerializeField] private GameObject SkillHeaderPrefab;
    [SerializeField] private GameObject ActiveSkills;
    [SerializeField] private GameObject PassiveSkills;
    [SerializeField] private GameObject skillTreeTabOutline;
    [SerializeField] private GameObject skillTreePage;
    [SerializeField] private GameObject equippedSkillsTabOutline;
    [SerializeField] private GameObject equippedSkillsPage;
    [SerializeField] private GameObject emptyDescriptionObj;
    [SerializeField] private GameObject descriptionObj;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void OnEnable()
    {
        for (int i = 0; i < skillsDataBase.GetCount(); i++)
        {
            var tmpSkill = skillsDataBase.GetSkill(i); 
            if (tmpSkill.type == SkillData.SkillType.Passive && tmpSkill.status == SkillData.SkillStatus.Learned)
            {
                GameObject skillHeader = Instantiate(SkillHeaderPrefab, Vector3.zero, Quaternion.identity, transform);
                skillHeader.transform.SetParent(PassiveSkills.transform, false);

                skillHeader.GetComponent<SkillHeaderManager>().ChosenSkill = tmpSkill;
                skillHeader.GetComponent<SkillHeaderManager>().SkillListManager = gameObject;
                skillHeader.GetComponent<SkillHeaderManager>().UpdateHeader();
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

    public void OpenSkillTree()
    {
        equippedSkillsPage.SetActive(false);
        skillTreePage.SetActive(true);
        
        equippedSkillsTabOutline.SetActive(false);
        skillTreeTabOutline.SetActive(true);
    }
}
