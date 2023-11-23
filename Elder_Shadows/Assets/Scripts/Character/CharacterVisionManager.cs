using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterVisionManager : MonoBehaviour
{
    public Button interact;
    private List<GameObject> visible = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        visible.Add(other.gameObject);
        if (other.CompareTag("Interactable"))
        {
            ChangeInteractButtonState(true, () => other.gameObject.GetComponent<IInteractable>().Interact());
            Debug.Log("Colliding with " + other.gameObject.name + " interactable");   
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        visible.Remove(other.gameObject);
        if (other.CompareTag("Interactable"))
        {
            bool canInteract = false;
            UnityAction action = null;
            foreach (var obj in visible)
            {
                if (obj.CompareTag("Interactable"))
                {
                    canInteract = true;
                    action = () => obj.GetComponent<IInteractable>().Interact();
                    break;
                }
            }
            ChangeInteractButtonState(canInteract, action);
        }
    }

    private void ChangeInteractButtonState(bool state, UnityAction action)
    {
        if (state)
        {
            interact.onClick.AddListener(action);
        }
        else
        {
            interact.onClick.RemoveAllListeners();
        }
        interact.gameObject.SetActive(state);
    }
}
