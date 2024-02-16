using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public event Action<string, Action> OnShowReplic;

    [SerializeField] private DialogLinesSO dialogLines;

    protected bool isInteracting = false;
    protected bool canInteract = true;

    public virtual bool CanInteract()
    {
        return canInteract;
    }

    public virtual void Interact()
    {
        if (dialogLines == null || dialogLines.lines.Length == 0) return;
        isInteracting = true;
        canInteract = false;
        string replic = dialogLines.lines[UnityEngine.Random.Range(0, dialogLines.lines.Length)];
        OnShowReplic?.Invoke(replic, FinishInteraction);
    }

    private void FinishInteraction()
    {
        isInteracting = false;
        canInteract = true;
    }
}
