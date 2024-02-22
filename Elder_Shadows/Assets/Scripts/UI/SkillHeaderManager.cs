using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class SkillHeaderManager : MonoBehaviour
{
    public bool isChosen = false;
    
    [SerializeField] private GameObject emptyDescription;
    [SerializeField] private GameObject description;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject unequipBtn;
    
    public void Choose()
    {
        if (!description.activeSelf)
        {
            emptyDescription.SetActive(false);
            description.SetActive(true);
        }
            
        if (!isChosen)
        {
            isChosen = true;
            gameObject.GetComponent<Image>().color = new Color(0.52f, 0.35f, 0.24f, 0.54f);
            unequipBtn.SetActive(true);
        }
        else
        {
            isChosen = false;
            gameObject.GetComponent<Image>().color = new Color(0.35f, 0.22f, 0.16f, 0.54f);
            unequipBtn.SetActive(false);
        }
    }
}
