using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseSkillSlotManager : MonoBehaviour
{
    public SkillData ChosenSkill;
    public GameObject SkillSlotsListManager;
    public int index = -1;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private GameObject skillIcon;
    
    public void Choose()
    {
        SkillSlotsListManager.GetComponent<SkillSlotListManager>().ChooseSlot(gameObject);
        SkillSlotsListManager.SetActive(false);
    }

    public void UpdateHeader()
    {
        if (ChosenSkill != null)
        {
            skillName.text = ChosenSkill.name;
        }
        else
        {
            skillName.text = "empty";
        }
    }
}
