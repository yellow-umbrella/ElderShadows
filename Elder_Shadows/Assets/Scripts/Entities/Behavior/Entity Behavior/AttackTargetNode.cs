using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AttackTargetNode : Node
{
    private BaseEntity entity;
    private float attackCooldown;
    private bool canAttack = true;
    public const string ATTACK_TARGET = "attack_target";

    public AttackTargetNode(BaseEntity entity, float attackCooldown)
    {
        this.entity = entity;
        this.attackCooldown = attackCooldown;
    }

    public override NodeState Evaluate()
    {
        GameObject target = (GameObject)GetData(ATTACK_TARGET);
        if (canAttack)
        {
            entity.Attack(target);
            entity.StartCoroutine(AttackCooldown());
        }
        state = NodeState.RUNNING;
        return state;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
