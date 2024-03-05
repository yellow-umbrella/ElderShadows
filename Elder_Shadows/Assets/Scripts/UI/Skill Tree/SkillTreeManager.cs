using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private GameObject LinesObj;
    [SerializeField] private TextMeshProUGUI SkillPointsCountText;
    [SerializeField] private SkillsDataBase skillsDataBase;
    
    public List<GameObject> skillNodes;
    public List<GameObject> skillLines;

    public int SkillPoints = 4;
    
    void Start()
    {
        InitiateNodes();
        DrawLines();
        RefreshPoints();
    }

    void InitiateNodes()
    {
        foreach (var node in skillNodes)
        {
            AddEvent(node, EventTriggerType.PointerUp, delegate { node.GetComponent<SkillNode>().ChooseSkillNode();});
            node.GetComponent<SkillNode>().DrawSkillNode();
            
            //прибрати потім, ініціалізація "пустого" дерева
            switch (node.GetComponent<SkillNode>().skillData.ID)
            {
                case 0:
                    node.GetComponent<SkillNode>().skillData.status = SkillData.SkillStatus.Learned;
                    break;
                case 1:
                    node.GetComponent<SkillNode>().skillData.status = SkillData.SkillStatus.Available;
                    break;
                case 3:
                    node.GetComponent<SkillNode>().skillData.status = SkillData.SkillStatus.Available;
                    break;
                default:
                    node.GetComponent<SkillNode>().skillData.status = SkillData.SkillStatus.Unavailable;
                    break;
            }
            node.GetComponent<SkillNode>().RefreshNode();
        }
        
        //теж видалити
        string s = "Skills IDs that are not in the Skill Tree: ", a = "", b = "";
        for (int i = 0; i < skillNodes.Count; i++)
        {
            a += skillNodes[i].GetComponent<SkillNode>().skillData.ID;
        }
        for (int j = 0; j < skillsDataBase.GetCount(); j++)
        {
            if (!a.Contains(j.ToString()))
            {
                s += j + ", ";
            }
        }
        Debug.Log(s);
    }

    void DrawLines()
    {
        skillLines = new List<GameObject>();
        foreach (var skillNode in skillNodes)
        {
            GameObject previousSkillNode = skillNode.GetComponent<SkillNode>().previousSkillNode;
            
            if (skillNode != previousSkillNode)
            {
                GameObject line = Instantiate(LinePrefab, Vector3.zero, Quaternion.identity, transform);
                RectTransform lineRT = line.GetComponent<RectTransform>();
                line.GetComponent<LineBetweenNodes>().ParentNode = previousSkillNode;
                line.GetComponent<LineBetweenNodes>().ChildNode = skillNode;
                
                line.transform.SetParent(LinesObj.transform, false);
                
                Vector2 posA = previousSkillNode.GetComponent<RectTransform>().anchoredPosition;
                Vector2 posB = skillNode.GetComponent<RectTransform>().anchoredPosition;
                Vector2 dir = (posA - posB).normalized;
                
                float distance = Vector2.Distance(posA, posB);
                lineRT.anchorMin = new Vector2(.5f, .5f);
                lineRT.anchorMax = new Vector2(.5f, .5f);
                lineRT.sizeDelta = new Vector2(distance, 5);
                lineRT.anchoredPosition = (posA + posB)/2;
                lineRT.eulerAngles = new Vector3(0, 0, GetEulerAngle(dir));
                
                skillLines.Add(line);
                
                Image lineImage = line.GetComponent<Image>();
                SkillData.SkillStatus statusChild = skillNode.GetComponent<SkillNode>().skillData.status;
                SkillData.SkillStatus statusParent = previousSkillNode.GetComponent<SkillNode>().skillData.status;
                if (statusParent == SkillData.SkillStatus.Unavailable || statusParent == SkillData.SkillStatus.Available)
                    lineImage.color =  new Color(.3f, .3f, .3f, 1f);
                else if (statusChild == SkillData.SkillStatus.Available)
                    lineImage.color =  new Color(.83f, .69f, .45f, 1f);
                else 
                    lineImage.color =  new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    public void RefreshLines()
    {
        foreach (var line in skillLines)
        {
            Image lineImage = line.GetComponent<Image>();
            
            SkillData.SkillStatus statusChild = line.GetComponent<LineBetweenNodes>().ChildNode.GetComponent<SkillNode>().skillData.status;
            SkillData.SkillStatus statusParent = line.GetComponent<LineBetweenNodes>().ParentNode.GetComponent<SkillNode>().skillData.status;
            if (statusParent == SkillData.SkillStatus.Unavailable || statusParent == SkillData.SkillStatus.Available)
                lineImage.color =  new Color(.3f, .3f, .3f, 1f);
            else if (statusChild == SkillData.SkillStatus.Available)
                lineImage.color =  new Color(.83f, .69f, .45f, 1f);
            else 
                lineImage.color =  new Color(1f, 1f, 1f, 1f);
        }
    }

    public void RefreshNodes(GameObject changedSkillNode)
    {
        for (int i = skillNodes.Count - 1; i >= 0; i--)
        {
            var tmpNode = skillNodes[i].GetComponent<SkillNode>();
            if (tmpNode.previousSkillNode == changedSkillNode && changedSkillNode.GetComponent<SkillNode>().skillData.ID != 0 &&
                tmpNode.skillData.status != SkillData.SkillStatus.Equipped && tmpNode.skillData.status != SkillData.SkillStatus.Learned)
            {
                switch (changedSkillNode.GetComponent<SkillNode>().skillData.status)
                {
                    case SkillData.SkillStatus.Learned:
                    {
                        tmpNode.skillData.status = SkillData.SkillStatus.Available;
                        break;
                    }
                    case SkillData.SkillStatus.Equipped:
                    {
                        tmpNode.skillData.status = SkillData.SkillStatus.Available;
                        break;
                    }
                }
            }
            skillNodes[i].GetComponent<SkillNode>().RefreshNode();
        }
    }

    public void ReduceSkillPoints()
    {
        SkillPoints--;
        RefreshPoints();
    }

    void RefreshPoints()
    {
        SkillPointsCountText.text = SkillPoints.ToString();
    }

    float GetEulerAngle(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI;
    }
    
    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
}
