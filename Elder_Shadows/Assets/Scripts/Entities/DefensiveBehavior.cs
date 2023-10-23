using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveBehavior : IBehavior
{
    private enum State
    {
        Wandering = 0,
        Attacking = 1,
    }
    private State state = State.Wandering;

    private GameObject target;
    private BaseEntity entity;

    public void Behave()
    {
        switch (state)
        {
            case State.Wandering:
                entity.WanderAround();
                break;
            case State.Attacking:
                if (target != null)
                {
                    entity.RunTowards(target.transform.position);
                    entity.TryAttack(target);
                }
                else
                {
                    entity.InitWandering(entity.transform.position);
                    state = State.Wandering;
                }
                break;
        }
    }

    public void InitBehavior(BaseEntity entity)
    {
        state = State.Wandering;
        this.entity = entity;
        entity.InitWandering(entity.transform.position);
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
