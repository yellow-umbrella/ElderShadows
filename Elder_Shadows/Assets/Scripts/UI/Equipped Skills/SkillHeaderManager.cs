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
            if (skillChosen)
            {
                isChosen = true;
                gameObject.GetComponent<Image>().color = new Color(0.52f, 0.35f, 0.24f, 0.54f);
                unequipBtn.SetActive(true);
            }
            else
            {
                SkillListManager.GetComponent<SkillListManager>().OpenSkillTree();
            }
        }
        else
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
    }
}
