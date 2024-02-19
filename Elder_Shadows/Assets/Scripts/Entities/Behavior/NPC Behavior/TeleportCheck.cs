using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TeleportCheck : Node
{
    private float probability;

    public TeleportCheck(float probability) 
    {
        this.probability = probability;
    }

    public override NodeState Evaluate()
    {
        if (Random.value < probability)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
