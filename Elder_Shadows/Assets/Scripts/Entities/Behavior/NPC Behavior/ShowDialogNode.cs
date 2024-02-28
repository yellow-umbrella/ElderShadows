using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ShowDialogNode : Node
{
    private NPC npc;
    private float timeToShow = 5f;
    private float timeToHide = 0f;

    public ShowDialogNode(NPC npc)
    {
        this.npc = npc;
    }

    public override NodeState Evaluate()
    {
        if ((Node)parent.GetData(PREV_ACTION_CHILD) != this)
        {
            if (npc.ShowBusyDialog())
            {
                timeToHide = Time.time + timeToShow;
                state = NodeState.RUNNING;
                return state;
            }
            npc.StopTalking();
            state = NodeState.FAILURE;
            return state;
        }
        if (Time.time >= timeToHide)
        {
            npc.StopTalking();
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.RUNNING;
        return state;
    }
}
