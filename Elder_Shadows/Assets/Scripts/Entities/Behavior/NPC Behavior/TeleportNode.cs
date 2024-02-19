using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;


public class TeleportNode : Node
{
    private NPC npc;

    public TeleportNode(NPC npc)
    {
        this.npc = npc;
    }

    public override NodeState Evaluate()
    {
        if (npc.Teleport())
        {
            UpdateCenterPosition();
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

    private void UpdateCenterPosition()
    {
        Node node = this;
        while (node.parent != null)
        {
            node = node.parent;
        }
        node.SetData(UpdatePathNode.CENTER_POSITION, (Vector2)npc.gameObject.transform.position);
    }
}
