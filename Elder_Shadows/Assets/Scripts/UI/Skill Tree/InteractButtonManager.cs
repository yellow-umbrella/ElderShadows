using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractButtonManager : MonoBehaviour
{
    [SerializeField] private SkillTreeManager skillTreeManager;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject noSkillPointsPopup;
    [SerializeField] private GameObject SkillSlots;
    public GameObject skillNode;
    
    public void Interact()
    {
        if (skillNode.GetComponent<SkillNode>().skillData.type == SkillData.SkillType.Active)
        {
            switch (skillNode.GetComponent<SkillNode>().skillData.status)
            {
                case SkillData.SkillStatus.Available:
                {
                    if (skillTreeManager.GetComponent<SkillTreeManager>().SkillPoints > 0)
                    {
                        skillNode.GetComponent<SkillNode>().skillData.Learn();
                        skillTreeManager.GetComponent<SkillTreeManager>().ReduceSkillPoints();
                    }
                    else
                    {
                        StartCoroutine(NotEnoughSkillPointsPopup());
                    }
                    break;
                }
                case SkillData.SkillStatus.Learned:
                {
                    SkillSlots.SetActive(true);
                    break;
                }
                case SkillData.SkillStatus.Equipped:
                {
                    skillNode.GetComponent<SkillNode>().skillData.Unequip();
                    skillTreeManager.skillsDataBase.EquippedSkills[Array.IndexOf(skillTreeManager.skillsDataBase.EquippedSkills, skillNode.GetComponent<SkillNode>().skillData)] = null;
                    break;
                }
            }
        }
        else if (skillNode.GetComponent<SkillNode>().skillData.status == SkillData.SkillStatus.Available)
        {
            if (skillTreeManager.GetComponent<SkillTreeManager>().SkillPoints > 0)
            {
                skillNode.GetComponent<SkillNode>().skillData.Learn();
                skillTreeManager.GetComponent<SkillTreeManager>().ReduceSkillPoints();
            }
            else
            {
                StartCoroutine(NotEnoughSkillPointsPopup());
            }
        }
        UpdateButton();
        skillTreeManager.RefreshNodes();
        skillTreeManager.RefreshLines();
    }

    public void UpdateButton()
    {
        switch (skillNode.GetComponent<SkillNode>().skillData.status)
        {
            case SkillData.SkillStatus.Unavailable:
                gameObject.SetActive(false);
                break;
            case SkillData.SkillStatus.Available:
            {
                gameObject.SetActive(true);
                buttonText.text = "Learn";
                break;
            }
            case SkillData.SkillStatus.Learned:
            {
                if (skillNode.GetComponent<SkillNode>().skillData.type == SkillData.SkillType.Active)
                {
                    gameObject.SetActive(true);
                    buttonText.text = "Equip";
                    break;
                }
                gameObject.SetActive(false);
                break;
            }
            case SkillData.SkillStatus.Equipped:
            {
                if (skillNode.GetComponent<SkillNode>().skillData.type == SkillData.SkillType.Active)
                {
                    gameObject.SetActive(true);
                    buttonText.text = "Unequip";
                    break;
                }
                gameObject.SetActive(false);
                break;
            }
        }
    }

    IEnumerator NotEnoughSkillPointsPopup()
    {
        noSkillPointsPopup.SetActive(true);

        yield return new WaitForSeconds(2f);
        
        noSkillPointsPopup.SetActive(false);
    }
}
