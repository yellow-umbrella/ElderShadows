using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TargetInAttackRangeCheck : Node
{
    private BaseEntity entity;

    public TargetInAttackRangeCheck(BaseEntity entity)
    {
        this.entity = entity;
    }

    public override NodeState Evaluate()
    {
        GameObject target = (GameObject)GetData(AttackTargetNode.ATTACK_TARGET);
        if (target != null && entity.CanAttack(target))
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
