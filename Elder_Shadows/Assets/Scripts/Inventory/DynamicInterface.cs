using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInterface : UserInterface
{
    public GameObject inventoryPrefab;
    public GameObject itemDescriptionPrefab;
    
    private Transform descriptionsBox;
    private float shiftY = -1;
    public override void CreateSlots()
    {
        descriptionsBox = transform.parent.Find("Descriptions").transform;
        
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        descriptionsOnInterface = new Dictionary<GameObject, GameObject>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().SetParent(gameObject.transform);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.PointerDown, delegate { OnDown(obj); });
            AddEvent(obj, EventTriggerType.PointerUp, delegate { OnUp(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            inventory.GetSlots[i].slotDisplay = obj;
            OnSlotUpdate(inventory.GetSlots[i]);

            slotsOnInterface.Add(obj, inventory.GetSlots[i]);
            
            StartCoroutine(CreateDescription(obj));
        }
    }

    IEnumerator CreateDescription(GameObject obj)
    {
        yield return new WaitForSeconds(.1f);
            
        var desc = Instantiate(itemDescriptionPrefab, Vector3.zero, Quaternion.identity, descriptionsBox.transform);
        var descRT = desc.GetComponent<RectTransform>();
        var contentRT = transform.parent.Find("Content").GetComponent<RectTransform>();
        descRT.SetParent(descriptionsBox.transform);

        descRT.anchorMin =
            new Vector2(Mathf.Abs((obj.GetComponent<RectTransform>().localPosition.x + contentRT.sizeDelta.x / 2) / contentRT.sizeDelta.x),
                1 - Mathf.Abs(obj.GetComponent<RectTransform>().localPosition.y / contentRT.sizeDelta.y));
        descRT.anchorMax = descRT.anchorMin;
        
        if (shiftY == -1)
            shiftY = Mathf.Abs(obj.GetComponent<RectTransform>().localPosition.y) -
                       (descRT.sizeDelta.y / 2 - itemDescriptionPrefab.transform.Find("Description").GetComponent<RectTransform>().sizeDelta.y); 
        
        descRT.anchoredPosition = new Vector2(0, shiftY);
        desc.GetComponent<ItemDescriptionManager>().inventorySlot = slotsOnInterface[obj];
            
        descriptionsOnInterface.Add(obj, desc);
        
        
    }
}
