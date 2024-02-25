using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class ChooseNormalAttackNode : Node
{
    private BaseEntity entity;
    private bool canAttack = true;

    public ChooseNormalAttackNode(BaseEntity entity)
    {
        this.entity = entity;
    }

    public override NodeState Evaluate()
    {
        if (canAttack && entity.ChooseAttack(entity.Info.attacks))
        {
            GetRoot().SetData(AttackTargetNode.ATTACK_COOLDOWN, (Func<IEnumerator>)AttackCooldown);
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(entity.Info.attackCooldown);
        canAttack = true;
    }
}
