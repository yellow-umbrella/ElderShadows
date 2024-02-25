using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using System;

public class AttackTargetNode : Node
{
    private BaseEntity entity;
    public const string ATTACK_TARGET = "attack_target";
    public const string ATTACK_COOLDOWN = "attack_cooldown";

    public AttackTargetNode(BaseEntity entity)
    {
        this.entity = entity;
    }

    public override NodeState Evaluate()
    {
        GameObject target = GetData(ATTACK_TARGET) as GameObject;
        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (state == NodeState.RUNNING)
        {
            if (entity.CanInflictDamage)
            {
                entity.Attack(target);
                entity.CanInflictDamage = false;
            }
            if (!entity.IsAttacking)
            {
                state = NodeState.SUCCESS;
            }
            return state;
        }

        entity.StartAttack(target);
        if (GetData(ATTACK_COOLDOWN) is Func<IEnumerator> cooldown)
        {
            entity.StartCoroutine(cooldown());
        }
        state = NodeState.RUNNING;
        return state;
    }
}
