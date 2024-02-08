using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescriptionManager : MonoBehaviour
{
    private GameObject descriptionObj;
    private RectTransform descriptionObjRT;
    private float availableWidth = Screen.width, availableHeight = Screen.height;
    void Awake()
    {
        descriptionObj = transform.Find("Description").gameObject;
        descriptionObjRT = descriptionObj.GetComponent<RectTransform>();
        
        float CUIwidth = transform.root.Find("Canvas").Find("CharacterMenu").GetComponent<RectTransform>().sizeDelta.x;
        float CUIheight = transform.root.Find("Canvas").Find("CharacterMenu").GetComponent<RectTransform>().sizeDelta.y;
        float deltaX = (availableWidth - CUIwidth) / 2, deltaY = (availableHeight - CUIheight) / 2;
        
        availableWidth -= deltaX;
        availableHeight = deltaY;
    }

    public void OpenDescription()
    {
        gameObject.SetActive(true);
        AlignPosition();
    }

    private void AlignPosition()
    {
        if (descriptionObj.transform.position.x > availableWidth && descriptionObj.transform.position.y <= availableHeight)
        {
            descriptionObjRT.pivot = new Vector2(0, 1);
            descriptionObjRT.anchorMin = new Vector2(0, 1);
            descriptionObjRT.anchorMax = new Vector2(0, 1);
        }
        else if (descriptionObj.transform.position.x > availableWidth && descriptionObj.transform.position.y > availableHeight)
        {
            descriptionObjRT.pivot = new Vector2(0, 0);
            descriptionObjRT.anchorMin = new Vector2(0, 0);
            descriptionObjRT.anchorMax = new Vector2(0, 0);
        }
        else if (descriptionObj.transform.position.x <= availableWidth && descriptionObj.transform.position.y <= availableHeight)
        {
            descriptionObjRT.pivot = new Vector2(1, 0);
            descriptionObjRT.anchorMin = new Vector2(1, 0);
            descriptionObjRT.anchorMax = new Vector2(1, 0);
        }
    }
}