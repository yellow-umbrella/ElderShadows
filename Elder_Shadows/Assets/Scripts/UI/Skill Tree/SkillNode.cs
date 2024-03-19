using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{
    [SerializeField] private GameObject Frame;
    public SkillData skillData;
    public SkillNode ParentSkillNode;
    [SerializeField] private GameObject DescriptionArea;
    [SerializeField] private TextMeshProUGUI DescriptionHeader;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    [SerializeField] private TextMeshProUGUI DescriptionSkillType;

    public bool isChosen = false;
    public LinkedList<SkillNode> ChildSkillNodes = new LinkedList<SkillNode>();

    public void DrawSkillNode()
    {
        if (skillData.type == SkillData.SkillType.Passive)
        {
            var tmp = gameObject.GetComponent<RectTransform>().sizeDelta; 
            gameObject.GetComponent<RectTransform>().sizeDelta = tmp / 2;
        }
        RefreshNode();
        if (ParentSkillNode != this)
            ParentSkillNode.ChildSkillNodes.AddLast(this);
    }

    public void ChooseSkillNode()
    {
        if (!isChosen)
        {
            if (NodesData.PreviousChosenSkillNode != null)
            {
                NodesData.PreviousChosenSkillNode.GetComponent<SkillNode>().isChosen = false;
                NodesData.PreviousChosenSkillNode.GetComponent<SkillNode>().RefreshNode();
            }
            isChosen = true;
            DescriptionArea.GetComponent<SkillTreeDescriptionManager>().ChosenSkillNode = gameObject;
            DescriptionArea.GetComponent<SkillTreeDescriptionManager>().Activate();
            DescriptionHeader.text = skillData.name;
            DescriptionText.text = skillData.description;
            DescriptionSkillType.text = skillData.type.ToString();
            RefreshNode();
            NodesData.PreviousChosenSkillNode = gameObject;
        }
    }

    public void RefreshNode()
    {
        if (isChosen)
        {
            if (NodesData.PreviousChosenSkillNode != null)
                NodesData.PreviousChosenSkillNode.GetComponent<SkillNode>().Frame.SetActive(false);
            Frame.SetActive(true);
        }
        else
        {
            Frame.SetActive(false);
        }

        if (ParentSkillNode.skillData.status == SkillData.SkillStatus.Unavailable || ParentSkillNode.skillData.status == SkillData.SkillStatus.Available)
        {
            skillData.status = SkillData.SkillStatus.Unavailable;
        } else if ((ParentSkillNode.skillData.status == SkillData.SkillStatus.Learned ||
                    ParentSkillNode.skillData.status == SkillData.SkillStatus.Equipped) &&
                   skillData.status == SkillData.SkillStatus.Unavailable)
        {
            Debug.Log("changed available");
            skillData.status = SkillData.SkillStatus.Available;
        }
        
        switch (skillData.status)
        {
            case SkillData.SkillStatus.Unavailable:
                gameObject.GetComponent<Image>().color = new Color(.3f, .3f, .3f, 1f);
                break;
            case SkillData.SkillStatus.Available:
                gameObject.GetComponent<Image>().color = new Color(.7f, .7f, .7f, 1f);
                break;
            case SkillData.SkillStatus.Learned:
                gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                break;
            case SkillData.SkillStatus.Equipped:
                gameObject.GetComponent<Image>().color = new Color(.83f, .69f, .45f, 1f);
                break;
        }
    }
}

public static class NodesData
{
    public static GameObject PreviousChosenSkillNode;
}
