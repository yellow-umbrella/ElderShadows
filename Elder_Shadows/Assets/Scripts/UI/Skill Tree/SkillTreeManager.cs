using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private GameObject LinesObj;
    
    public List<GameObject> skillNodes;
    private List<GameObject> skillLines;

    void Start()
    {
        DrawLines();
    }

    void DrawLines()
    {
        foreach (var skillNode in skillNodes)
        {
            GameObject previousSkillNode = skillNode.GetComponent<SkillNode>().previousSkillNode;
            
            if (skillNode != previousSkillNode)
            {
                Debug.Log("second");
                GameObject line = Instantiate(LinePrefab, Vector3.zero, Quaternion.identity, transform);
                RectTransform lineRT = line.GetComponent<RectTransform>();
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
            }
        }
    }

    float GetEulerAngle(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI;
    }
}
