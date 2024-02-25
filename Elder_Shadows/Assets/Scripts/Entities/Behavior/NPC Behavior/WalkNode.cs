using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class WalkNode : Node
{
    private MovementController controller;
    public const string FINISH_WALK = "finish_walk";

    public WalkNode(MovementController movementController)
    {
        controller = movementController;
    }

    public override NodeState Evaluate()
    {
        object finishWalk = GetData(FINISH_WALK);
        if (controller.ReachedEndOfPath || (finishWalk != null && (bool)finishWalk))
        {
            state = NodeState.SUCCESS;
            return state;
        }
        controller.MoveOnPath();
        state = NodeState.RUNNING;
        return state;
    }
}
