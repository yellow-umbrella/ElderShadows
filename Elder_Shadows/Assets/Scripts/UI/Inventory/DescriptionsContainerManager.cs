using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionsContainerManager : MonoBehaviour
{
    private RectTransform contentRT;
    private RectTransform descriptionContainterRT;

    void Start()
    {
        descriptionContainterRT = gameObject.GetComponent<RectTransform>();
        contentRT = transform.parent.Find("Content").GetComponent<RectTransform>();
    }

    public void UpdateDescriptionContainers()
    {
        descriptionContainterRT.sizeDelta = contentRT.sizeDelta;
    }
}
