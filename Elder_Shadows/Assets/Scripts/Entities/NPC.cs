using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public event Action<string, Action> OnShowReplic;

    [SerializeField] private DialogLinesSO dialogLines;
    [SerializeField] private Vector2 positionLimits;

    protected bool isInteracting = false;
    protected bool canInteract = true;

    public virtual bool CanInteract()
    {
        return canInteract;
    }

    public virtual void Interact()
    {
        isInteracting = true;
    }

    protected void FinishInteraction()
    {
        isInteracting = false;
    }

    public bool ShowDialog()
    {
        if (dialogLines == null || dialogLines.lines.Length == 0)
        {
            FinishInteraction();
            return false;
        }
        string replic = dialogLines.lines[UnityEngine.Random.Range(0, dialogLines.lines.Length)];
        OnShowReplic?.Invoke(replic, FinishInteraction);
        return true;
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }

    public bool Teleport()
    {
        Vector2 position = transform.position;
        if (EntitySpawner.Instance.GetSafePosition(positionLimits, ref position))
        {
            transform.position = position;
            return true;
        }
        return false;
    }
}
