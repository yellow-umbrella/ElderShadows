using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class SkillHeaderManager : MonoBehaviour
{
    public bool isChosen = false;
    public SkillData ChosenSkill;
    public GameObject SkillListManager;
    public int index = -1;

    [SerializeField] private GameObject unequipBtn;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private GameObject skillIcon;
    private bool skillChosen = false;
    
    public void Choose()
    {
        UpdateHeader();
        SkillListManager.GetComponent<SkillListManager>().ChooseHeader(gameObject);
        
        if (!isChosen)
        {
            if (skillChosen && SkillListData.PreviousHeader != gameObject)
            {
                isChosen = true;
                gameObject.GetComponent<Image>().color = new Color(0.52f, 0.35f, 0.24f, 0.54f);
                unequipBtn.SetActive(true);

                if (SkillListData.PreviousHeader == null)
                {
                    SkillListData.PreviousHeader = gameObject;
                }
                else
                {
                    SkillListData.PreviousHeader.GetComponent<SkillHeaderManager>().Unchoose();
                    SkillListData.PreviousHeader = gameObject;
                }
            }
            else
            {
                SkillListManager.GetComponent<SkillListManager>().OpenSkillTree();
            }
        }
    }

    public void UnequipSkill()
    {
        if (ChosenSkill.status == SkillData.SkillStatus.Equipped)
        {
            ChosenSkill = null;
            SkillListManager.GetComponent<SkillListManager>().UnequipSkill(index);
            UpdateHeader();
            Unchoose();
        }
    }

    void Unchoose()
    {
        if (isChosen)
        {
            isChosen = false;
            gameObject.GetComponent<Image>().color = new Color(0.35f, 0.22f, 0.16f, 0.54f);
            unequipBtn.SetActive(false);
        }
    }

    public void UpdateHeader()
    {
        if (ChosenSkill != null)
        {
            skillChosen = true;
            skillName.text = ChosenSkill.name;
        }
        else
        {
            skillChosen = false;
            skillName.text = "empty";
        }
    }
}
