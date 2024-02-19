using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDescriptionManager : MonoBehaviour
{
    public InventorySlot inventorySlot;

    [SerializeField] private GameObject descriptionObj;
    [SerializeField] private RectTransform contentRT;
    [SerializeField] private TextMeshProUGUI itemNameTMP;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;
    private RectTransform descriptionObjRT;
    private RectTransform descriptionBoxRT;
    private Vector2 availableAnchorArea;
    private float availableWidth, availableHeight;

    void Awake()
    {
        descriptionObjRT = descriptionObj.GetComponent<RectTransform>();
        descriptionBoxRT = gameObject.GetComponent<RectTransform>();

        Vector2 contentSizeDelta = contentRT.sizeDelta;

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
        itemNameTMP.text = inventorySlot.ItemObject.name;
        itemDescriptionTMP.text = inventorySlot.ItemObject.description;
    }
    
    private void AlignPosition()
    {
        ChangePosition();
    }

    private void ChangePosition()
    {
        if (descriptionBoxRT.anchorMax.x > availableAnchorArea.x && descriptionBoxRT.anchorMax.y > availableAnchorArea.y) // crosses right
        {
            descriptionObjRT.pivot = new Vector2(0, 0);
            descriptionObjRT.anchorMin = new Vector2(0, 0);
            descriptionObjRT.anchorMax = new Vector2(0, 0);
            
            if (descriptionBoxRT.anchoredPosition.y < 0)
                descriptionBoxRT.anchoredPosition = new Vector2(descriptionBoxRT.anchoredPosition.x, -descriptionBoxRT.anchoredPosition.y);
        }
        else if (descriptionBoxRT.anchorMax.x <= availableAnchorArea.x && descriptionBoxRT.anchorMax.y <= availableAnchorArea.y) // crosses bottom
        {
            descriptionObjRT.pivot = new Vector2(1, 1);
            descriptionObjRT.anchorMin = new Vector2(1, 1);
            descriptionObjRT.anchorMax = new Vector2(1, 1);
            
            if (descriptionBoxRT.anchoredPosition.y > 0)
                descriptionBoxRT.anchoredPosition = new Vector2(descriptionBoxRT.anchoredPosition.x, -descriptionBoxRT.anchoredPosition.y);
        }
        else if (descriptionBoxRT.anchorMax.x > availableAnchorArea.x && descriptionBoxRT.anchorMax.y <= availableAnchorArea.y) // crosses bottom right
        {
            descriptionObjRT.pivot = new Vector2(0, 1);
            descriptionObjRT.anchorMin = new Vector2(0, 1);
            descriptionObjRT.anchorMax = new Vector2(0, 1);
            
            if (descriptionBoxRT.anchoredPosition.y > 0)
                descriptionBoxRT.anchoredPosition = new Vector2(descriptionBoxRT.anchoredPosition.x, -descriptionBoxRT.anchoredPosition.y);
        }
        else if (descriptionBoxRT.anchorMax.x <= availableAnchorArea.x && descriptionBoxRT.anchorMax.y > availableAnchorArea.y) // default (doesnt cross)
        {
            descriptionObjRT.pivot = new Vector2(1, 0);
            descriptionObjRT.anchorMin = new Vector2(1, 0);
            descriptionObjRT.anchorMax = new Vector2(1, 0);
            
            if (descriptionBoxRT.anchoredPosition.y < 0)
                descriptionBoxRT.anchoredPosition = new Vector2(descriptionBoxRT.anchoredPosition.x, -descriptionBoxRT.anchoredPosition.y);
        }
    }
}