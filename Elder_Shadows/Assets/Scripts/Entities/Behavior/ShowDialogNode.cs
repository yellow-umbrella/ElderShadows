using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ShowDialogNode : Node
{
    private NPC npc;

    public ShowDialogNode(NPC npc)
    {
        this.npc = npc;
    }

    public override NodeState Evaluate()
    {
        if ((Node)parent.GetData(PREV_ACTION) != this)
        {
            if (!npc.ShowDialog())
            {
                state = NodeState.FAILURE;
                return state;
            }
        }
        state = NodeState.RUNNING; 
        return state;
    }
}
