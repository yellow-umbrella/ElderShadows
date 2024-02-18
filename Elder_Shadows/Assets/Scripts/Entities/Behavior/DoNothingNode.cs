using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class DoNothingNode : Node
{
    public override NodeState Evaluate()
    {
        state = NodeState.SUCCESS;
        return state;
    }
}
