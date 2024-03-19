using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillTreeDescriptionManager : MonoBehaviour
{
    public GameObject ChosenSkillNode;
    [SerializeField] private GameObject InteractButton;
    [SerializeField] private TextMeshProUGUI InteractButtonText;

    public void Activate()
    {
        gameObject.SetActive(true);
        InteractButton.GetComponent<InteractButtonManager>().skillNode = ChosenSkillNode;
        InteractButton.GetComponent<InteractButtonManager>().UpdateButton();
    }

    public void UpdateButton()
    {
        switch (ChosenSkillNode.GetComponent<SkillNode>().skillData.status)
        {
            case SkillData.SkillStatus.Unavailable:
                InteractButton.SetActive(false);
                break;
            case SkillData.SkillStatus.Available:
            {
                InteractButton.SetActive(true);
                InteractButtonText.text = "Learn";
                break;
            }
            case SkillData.SkillStatus.Learned:
            {
                if (ChosenSkillNode.GetComponent<SkillNode>().skillData.type == SkillData.SkillType.Active)
                {
                    InteractButton.SetActive(true);
                    InteractButtonText.text = "Equip";
                    break;
                }
                InteractButton.SetActive(false);
                break;
            }
            case SkillData.SkillStatus.Equipped:
            {
                if (ChosenSkillNode.GetComponent<SkillNode>().skillData.type == SkillData.SkillType.Active)
                {
                    InteractButton.SetActive(true);
                    InteractButtonText.text = "Unequip";
                    break;
                }
                InteractButton.SetActive(false);
                break;
            }
        }
    }

    public void SetInactive()
    {
        ChosenSkillNode.GetComponent<SkillNode>().isChosen = false;
        ChosenSkillNode.GetComponent<SkillNode>().RefreshNode();
        gameObject.SetActive(false);
    }
}
