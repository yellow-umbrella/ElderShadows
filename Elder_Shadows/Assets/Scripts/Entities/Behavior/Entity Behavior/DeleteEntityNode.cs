using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class DeleteEntityNode : Node
{
    private BaseEntity entity;

    public DeleteEntityNode(BaseEntity entity)
    {
        this.entity = entity;
    }

    public override NodeState Evaluate()
    {
        EntitySpawner.Instance.DeleteEntity(entity);
        state = NodeState.SUCCESS;
        return state;
    }
}
