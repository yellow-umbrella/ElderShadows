using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TargetInSeeingRangeCheck : Node
{
    private BaseEntity entity;
    private float checkCooldown = .5f;
    private bool canCheck = true;
    public const string FOCUSED_ATTACK = "focused_attack";

    public TargetInSeeingRangeCheck(BaseEntity entity)
    {
        this.entity = entity;
    }

    public override NodeState Evaluate()
    {
        // if was hit by target and see it, than go to it
        GameObject hitBy = entity.HitByTarget();
        if (hitBy != null && hitBy.GetComponent<IAttackable>() != null && entity.CanSee(hitBy))
        {
            GetRoot().SetData(AttackTargetNode.ATTACK_TARGET, hitBy);
            state = NodeState.SUCCESS;
            return state;
        }

        // if have current target and can see it, than go to it
        GameObject target = (GameObject)GetData(AttackTargetNode.ATTACK_TARGET);
        object isFocusedAttack = GetData(FOCUSED_ATTACK);
        if (target != null && (entity.CanSee(target) || isFocusedAttack is bool && (bool)isFocusedAttack))
        {
            state = NodeState.SUCCESS;
            return state;
        }

        // find new aggression targets
        if (canCheck)
        {
            List<GameObject> targets = entity.FindTargets();
            entity.StartCoroutine(CheckCooldown());

            if (targets.Count == 0)
            {
                GetRoot().SetData(AttackTargetNode.ATTACK_TARGET, null);
                state = NodeState.FAILURE;
                return state;
            }

            // select the nearest target
            target = null;
            float minDist = float.PositiveInfinity;
            foreach (GameObject gameObject in targets)
            {
                float dist = Vector3.Distance(gameObject.transform.position, entity.gameObject.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = gameObject;
                }
            }

            GetRoot().SetData(AttackTargetNode.ATTACK_TARGET, target);
            state = NodeState.SUCCESS;
            return state;
        }

        GetRoot().SetData(AttackTargetNode.ATTACK_TARGET, null);
        state = NodeState.FAILURE;
        return state;
    }

    private IEnumerator CheckCooldown()
    {
        canCheck = false;
        yield return new WaitForSeconds(checkCooldown);
        canCheck = true;
    }
}
