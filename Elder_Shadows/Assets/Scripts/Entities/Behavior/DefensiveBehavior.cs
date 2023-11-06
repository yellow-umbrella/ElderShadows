using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveBehavior : IAttackBehavior
{
    private enum State
    {
        Idle = 0,
        Attacking = 1,
    }
    private State state;

    private GameObject target;
    private BaseEntity entity;

    public DefensiveBehavior(BaseEntity entity)
    {
        this.entity = entity;
        state = State.Idle;
    }

    public bool Behave()
    {
        if (state == State.Attacking)
        {
            if (target != null)
            {
                if (!entity.TryAttack(target))
                {
                    entity.MoveTowards(target.transform.position);
                }
                return true;
            }
            else
            {
                state = State.Idle;
            }
        }
        return false;
    }

    public void OnHit(GameObject other)
    {
        float distTarget = float.PositiveInfinity;
        float distOther = Vector3.Distance(other.transform.position, entity.transform.position);
        if (target != null)
        {
            distTarget = Vector3.Distance(target.transform.position, entity.transform.position);
        }

        if (other.HasComponent<IAttackable>() && distOther <= distTarget)
        {
            target = other;
            state = State.Attacking;
        }
    }

    public void OnSee(GameObject other)
    {
        //ignore
    }
}
