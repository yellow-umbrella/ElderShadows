using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class SkillHeaderManager : MonoBehaviour
{
    public bool isChosen = false;
    
    [SerializeField] private GameObject skillTreeTabOutline;
    [SerializeField] private GameObject skillTreePage;
    [SerializeField] private GameObject equippedSkillsTabOutline;
    [SerializeField] private GameObject equippedSkillsPage;
    [SerializeField] private GameObject emptyDescriptionObj;
    [SerializeField] private GameObject descriptionObj;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject unequipBtn;
    [SerializeField] private bool skillChosen = false;
    
    public void Choose()
    {
        if (!descriptionObj.activeSelf)
        {
            emptyDescriptionObj.SetActive(false);
            descriptionObj.SetActive(true);
        }
        
        if (!isChosen)
        {
            Debug.Log(skillChosen);
            if (skillChosen)
            {
                isChosen = true;
                gameObject.GetComponent<Image>().color = new Color(0.52f, 0.35f, 0.24f, 0.54f);
                unequipBtn.SetActive(true);
            }
            else
            {
                OpenSkillTree();
            }
        }
        else
        {
            isChosen = false;
            gameObject.GetComponent<Image>().color = new Color(0.35f, 0.22f, 0.16f, 0.54f);
            unequipBtn.SetActive(false);
        }
    }

    private void OpenSkillTree()
    {
        equippedSkillsPage.SetActive(false);
        skillTreePage.SetActive(true);
        
        equippedSkillsTabOutline.SetActive(false);
        skillTreeTabOutline.SetActive(true);
    }
}
