using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class PlayerInteractionCheck : Node
{
    private IInteractable interactable;

    public PlayerInteractionCheck(IInteractable interactable)
    {
        this.interactable = interactable;
    }

    public override NodeState Evaluate()
    {
        if (interactable.IsInteracting())
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
