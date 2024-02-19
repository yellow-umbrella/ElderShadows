using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject descriptionBoxDynamic;
    [SerializeField] private GameObject descriptionBoxStatic;

    void Start()
    {
        AddEvent(gameObject, EventTriggerType.PointerUp, delegate { OnUp(); });
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    private void OnUp()
    {
        descriptionBoxDynamic.SetActive(false);
        descriptionBoxStatic.SetActive(false);
    }
}
