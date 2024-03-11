using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TooFarCheck : Node
{
    private float distance;
    private Transform obj;

    public TooFarCheck(float distance, Transform obj)
    {
        this.distance = distance;
        this.obj = obj;
    }

    public override NodeState Evaluate()
    {
        if (Vector2.Distance(CharacterController.instance.transform.position, obj.position) > distance)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
