using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradingItemSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Image image;
    [SerializeField] private Image background;
    public TradingUIManager Manager { get; set; }
    private ItemObject item;

    private readonly Color backgroundColor = new Color(.25f, .2f, .2f, 1);
    private readonly Color selectedBackgroundColor = new Color(.5f, .4f, .29f, 1);

    private void Awake()
    {
        AddEvent(gameObject, EventTriggerType.PointerClick, delegate { OnSelect(); });
        background.color = backgroundColor;
    }

    public void UpdateSlot(ItemObject item, string price, string count)
    {
        if (item == null)
        {
            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
        } else
        {
            image.sprite = item.uiDisplay;
            image.color = new Color(1, 1, 1, 1);
        }
        this.price.text = price;
        this.count.text = count;
        this.item = item;
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnSelect()
    {
        if (item == null) { return; }
        background.color = selectedBackgroundColor;
        Manager.SelectItem(item, gameObject);
    }
    
    public void OnDeselect()
    {
        background.color = backgroundColor;
    }
}
