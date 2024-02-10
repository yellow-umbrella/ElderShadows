using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public abstract class UserInterface : MonoBehaviour
{
    public InventoryObject inventory;
    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
    public Dictionary<GameObject, GameObject> descriptionsOnInterface = new Dictionary<GameObject, GameObject>();
    void Start()
    {
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;

        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    public void OnSlotUpdate(InventorySlot _slot)
    {
        if (_slot.item.Id >= 0)
        {
            _slot.slotDisplay.transform.Find("Image").GetComponentInChildren<Image>().sprite = _slot.ItemObject.uiDisplay;
            _slot.slotDisplay.transform.Find("Image").GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
        }
        else
        {
            _slot.slotDisplay.transform.Find("Image").GetComponentInChildren<Image>().sprite = null;
            _slot.slotDisplay.transform.Find("Image").GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    slotsOnInterface.UpdateSlotDisplay();
    //}
    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
        
        transform.parent.Find("Descriptions").GetComponent<DescriptionsContainerManager>().UpdateDescriptionContainers();
    }
    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
    }
    
    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.2f;

    public void OnDown(GameObject obj)
    {
        if (MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver].ItemObject)
        {
            ClosePreviousDescription(obj);
        }
    }

    public void OnUp(GameObject obj)
    {
        if (Time.time - clicktime > clickdelay)
        {
            clicked = 0;
        }
        
        clicked++;

        if (clicked == 1)
        {
            clicktime = Time.time;
        
            if (MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver].ItemObject)
            {
                SetNewActiveSlot(obj);
            }
        }

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
        
            if (MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver].ItemObject)
            {
                OpenDescription(obj);
            }
        }
        else if (clicked > 2 || Time.time - clicktime > clickdelay)
        {
            clicked = 0;
        }
    }
    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }
    public void OnExitInterface(GameObject obj)
    {
        MouseData.previousInterfaceMouseWasOver = obj.GetComponent<UserInterface>();
        MouseData.interfaceMouseIsOver = null;
    }
    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }
    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if(slotsOnInterface[obj].item.Id >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;
            img.raycastTarget = false;
            img.maskable = false;
        }
        return tempItem;
    }
    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        if (MouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }
        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
            SetNewActiveSlot(MouseData.slotHoveredOver);
        }
    }
    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    
    
    
    private void SetNewActiveSlot(GameObject obj)
    {
        if (MouseData.activeSlot != null)
        {
            MouseData.activeSlot.transform.Find("ActiveFrame").gameObject.SetActive(false);
        }
        MouseData.activeSlot = obj;
        obj.transform.Find("ActiveFrame").gameObject.SetActive(true);
    }
    
    private void OpenDescription(GameObject obj)
    {
        ClosePreviousDescription(obj);
        MouseData.activeSlot = obj;
        
        MouseData.interfaceMouseIsOver.descriptionsOnInterface[obj].GetComponent<ItemDescriptionManager>().UpdateDescription();
        MouseData.interfaceMouseIsOver.descriptionsOnInterface[obj].GetComponent<ItemDescriptionManager>().OpenDescription();
    }

    private void ClosePreviousDescription(GameObject obj)
    {
        if (MouseData.activeSlot != null)
        {
            if (MouseData.interfaceMouseIsOver != MouseData.previousInterfaceMouseWasOver && MouseData.previousInterfaceMouseWasOver != null)
            {
                try
                {
                    MouseData.previousInterfaceMouseWasOver.descriptionsOnInterface[MouseData.activeSlot].gameObject.SetActive(false);
                    MouseData.previousInterfaceMouseWasOver = obj.GetComponent<UserInterface>();
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine(e);
                    MouseData.previousInterfaceMouseWasOver = obj.GetComponent<UserInterface>();
                }
            }
            else
            {
                MouseData.interfaceMouseIsOver.descriptionsOnInterface[MouseData.activeSlot].gameObject.SetActive(false);
            }
        }
    }
}
public static class MouseData
{
    public static UserInterface interfaceMouseIsOver;
    public static UserInterface previousInterfaceMouseWasOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
    public static GameObject activeSlot;
}


public static class ExtensionMethods
{
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
        {
            if (_slot.Value.item.Id >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.uiDisplay;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
}