using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class WalkNode : Node
{
    private MovementController controller;

    public WalkNode(MovementController movementController)
    {
        controller = movementController;
    }

    public override NodeState Evaluate()
    {
        controller.MoveOnPath();
        state = NodeState.RUNNING;
        return state;
    }
}
