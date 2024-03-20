using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    [SerializeField] public SkillsDataBase skillsDataBase;
    
    public List<GameObject> skillNodes;
    public List<GameObject> skillLines;

    public int SkillPoints = 4;
    
    void Start()
    {
        InitiateNodes();
        DrawLines();
        RefreshPoints();
        RefreshEquippedSkills();
    }

    void RefreshEquippedSkills()
    {
        for (int i = 0; i < skillsDataBase.EquippedSkills.Length; i++)
        {
            if (skillsDataBase.EquippedSkills[i] != null && skillsDataBase.EquippedSkills[i].status != SkillData.SkillStatus.Equipped)
                skillsDataBase.EquippedSkills[i] = null;
        }
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
                    node.GetComponent<SkillNode>().skillData.status = SkillData.SkillStatus.Equipped;
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
        
        skillsDataBase.EquippedSkills[0] = skillsDataBase.GetSkill(0);
    }

    private void OnEnable()
    {
        RefreshLines();
        RefreshNodes();
        RefreshPoints();
    }

    void DrawLines()
    {
        skillLines = new List<GameObject>();
        foreach (var skillNode in skillNodes)
        {
            SkillNode previousSkillNode = skillNode.GetComponent<SkillNode>().ParentSkillNode;
            
            if (skillNode != previousSkillNode)
            {
                GameObject line = Instantiate(LinePrefab, Vector3.zero, Quaternion.identity, transform);
                RectTransform lineRT = line.GetComponent<RectTransform>();
                line.GetComponent<LineBetweenNodes>().ParentNode = previousSkillNode.gameObject;
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

    public void RefreshNodes()
    {
        Queue<SkillNode> q = new Queue<SkillNode>();
        bool[] flag = new bool[skillNodes.Count];
        
        q.Enqueue(skillNodes[0].GetComponent<SkillNode>());
        flag[0] = true;

        while (q.Count > 0)
        {
            SkillNode w = q.Dequeue();
            w.RefreshNode();
            LinkedList<SkillNode> linkedNodes = w.ChildSkillNodes;
            LinkedListNode<SkillNode> currentNode = linkedNodes.First;
            while (currentNode != null && !flag[skillNodes.IndexOf(currentNode.Value.gameObject)])
            {
                q.Enqueue(currentNode.Value);
                flag[skillNodes.IndexOf(currentNode.Value.gameObject)] = true;
                currentNode = currentNode.Next;
            }
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