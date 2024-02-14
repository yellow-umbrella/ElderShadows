using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDescriptionManager : MonoBehaviour
{
    public InventorySlot inventorySlot;

    private GameObject descriptionObj;
    private RectTransform descriptionObjRT;
    private RectTransform descriptionBoxRT;
    private Vector2 availableAnchorArea;
    private float availableWidth, availableHeight;

    void Awake()
    {
        descriptionObj = transform.Find("Description").gameObject;
        descriptionObjRT = descriptionObj.GetComponent<RectTransform>();
        descriptionBoxRT = gameObject.GetComponent<RectTransform>();

        Vector2 contentSizeDelta = transform.parent.parent.Find("Content").GetComponent<RectTransform>().sizeDelta;

        availableAnchorArea.x = 1 - descriptionBoxRT.sizeDelta.x / 2 / contentSizeDelta.x;
        availableAnchorArea.y = 1 - descriptionBoxRT.sizeDelta.y / 2 / contentSizeDelta.y;
    }

    public void OpenDescription()
    {
        UpdateDescription();
        gameObject.SetActive(true);
        AlignPosition();
    }
    
    public void UpdateDescription()
    {
        transform.Find("Description").Find("ItemName").GetComponent<TextMeshProUGUI>().text = inventorySlot.ItemObject.name;
        transform.Find("Description").Find("ItemDescription").GetComponent<TextMeshProUGUI>().text = inventorySlot.ItemObject.description;
    }
    
    private void AlignPosition()
    {
        ChangePosition();
    }

    private void ChangePosition()
    {
        if (descriptionBoxRT.anchorMax.x > availableAnchorArea.x && descriptionBoxRT.anchorMax.y > availableAnchorArea.y) // right
        {
            descriptionObjRT.pivot = new Vector2(0, 0);
            descriptionObjRT.anchorMin = new Vector2(0, 0);
            descriptionObjRT.anchorMax = new Vector2(0, 0);
            
            if (descriptionBoxRT.anchoredPosition.y < 0)
                descriptionBoxRT.anchoredPosition = new Vector2(descriptionBoxRT.anchoredPosition.x, -descriptionBoxRT.anchoredPosition.y);
        }
        else if (descriptionBoxRT.anchorMax.x <= availableAnchorArea.x && descriptionBoxRT.anchorMax.y <= availableAnchorArea.y) // bottom
        {
            descriptionObjRT.pivot = new Vector2(1, 1);
            descriptionObjRT.anchorMin = new Vector2(1, 1);
            descriptionObjRT.anchorMax = new Vector2(1, 1);
            
            if (descriptionBoxRT.anchoredPosition.y > 0)
                descriptionBoxRT.anchoredPosition = new Vector2(descriptionBoxRT.anchoredPosition.x, -descriptionBoxRT.anchoredPosition.y);
        }
        else if (descriptionBoxRT.anchorMax.x > availableAnchorArea.x && descriptionBoxRT.anchorMax.y <= availableAnchorArea.y) // bottom right
        {
            descriptionObjRT.pivot = new Vector2(0, 1);
            descriptionObjRT.anchorMin = new Vector2(0, 1);
            descriptionObjRT.anchorMax = new Vector2(0, 1);
            
            if (descriptionBoxRT.anchoredPosition.y > 0)
                descriptionBoxRT.anchoredPosition = new Vector2(descriptionBoxRT.anchoredPosition.x, -descriptionBoxRT.anchoredPosition.y);
        }
        else if (descriptionBoxRT.anchorMax.x <= availableAnchorArea.x && descriptionBoxRT.anchorMax.y > availableAnchorArea.y) // default
        {
            descriptionObjRT.pivot = new Vector2(1, 0);
            descriptionObjRT.anchorMin = new Vector2(1, 0);
            descriptionObjRT.anchorMax = new Vector2(1, 0);
            
            if (descriptionBoxRT.anchoredPosition.y < 0)
                descriptionBoxRT.anchoredPosition = new Vector2(descriptionBoxRT.anchoredPosition.x, -descriptionBoxRT.anchoredPosition.y);
        }
        //descriptionBoxRT.anchoredPosition = new Vector2(0, 0);
    }
}