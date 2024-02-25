using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using System;

public class ChooseSpecialAttackNode : Node
{
    private BaseEntity entity;
    private bool canAttack = true;

    public ChooseSpecialAttackNode(BaseEntity entity)
    {
        this.entity = entity;
    }

    public override NodeState Evaluate()
    {
        if (canAttack && UnityEngine.Random.value < entity.Info.specialAttackChance && entity.ChooseAttack(entity.Info.specialAttacks))
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
        yield return new WaitForSeconds(entity.Info.specialAttackCooldown);
        canAttack = true;
    }
}
