using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class InvisibilityCheck : Node
{
    private Transform obj;
    private float checkOffset = 1;

    public InvisibilityCheck(Transform obj)
    {
        this.obj = obj;
    }

    public override NodeState Evaluate()
    {
        Vector2 viewportPostion = Camera.main.WorldToViewportPoint(obj.position);
        if (Mathf.Max(viewportPostion.x, viewportPostion.y) <= 1 + checkOffset 
            && Mathf.Min(viewportPostion.x, viewportPostion.y) >= 0 - checkOffset)
        {
            state = NodeState.FAILURE;
            return state;
        }
        state = NodeState.SUCCESS;
        return state;
    }
}
