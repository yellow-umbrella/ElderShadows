using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public event Action<string> OnShowReplic;
    public event Action OnHideReplic;

    [SerializeField] private DialogLinesSO dialogLines;
    [SerializeField] private Vector2 positionLimits;
    [SerializeField] private Transform[] pointsToTeleport;

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

    public void FinishInteraction()
    {
        isInteracting = false;
    }

    public bool ShowBusyDialog()
    {
        return ShowDialog(dialogLines.busyLines);
    }

    public bool ShowGratitudeDialog()
    {
        return ShowDialog(dialogLines.gratitudeLines);
    }

    private bool ShowDialog(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            FinishInteraction();
            return false;
        }
        string replic = lines[UnityEngine.Random.Range(0, lines.Length)];
        OnShowReplic?.Invoke(replic);
        return true;
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }

    public void StopTalking()
    {
        OnHideReplic?.Invoke();
        FinishInteraction();
    }

    public bool Teleport()
    {
        // has predefined positions to teleport
        if (pointsToTeleport.Length > 0)
        {
            List<Vector2> validPositions = new List<Vector2>();
            foreach (var point in pointsToTeleport)
            {
                if (EntitySpawner.Instance.IsSafePosition(point.position))
                {
                    validPositions.Add(point.position);
                }
            }
            if (validPositions.Count == 0)
            {
                return false;
            }
            transform.position = validPositions[UnityEngine.Random.Range(0, validPositions.Count)];
            return true;
        }

        // teleports near the player
        Vector2 position = transform.position;
        if (EntitySpawner.Instance.GetSafePosition(positionLimits, ref position))
        {
            transform.position = position;
            return true;
        }
        return false;
    }
}
