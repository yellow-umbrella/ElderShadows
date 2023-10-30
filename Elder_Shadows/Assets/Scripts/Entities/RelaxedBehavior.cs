using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelaxedBehavior : IAttackBehavior
{
    private enum State
    {
        Idle = 0,
        RunningAway = 1,
    }
    private State state;
    private BaseEntity entity;
    private GameObject attacker;
    private const float MAX_DISTANCE = 100;

    public RelaxedBehavior(BaseEntity entity)
    {
        this.entity = entity;
        state = State.Idle;
    }

    public bool Behave()
    {
        if (state == State.RunningAway)
        {
            if (attacker != null && entity.CanSee(attacker))
            {
                Vector3 runningDirection = entity.transform.position - attacker.transform.position;
                if (runningDirection.ToVector2() == Vector2.zero)
                {
                    runningDirection = new Vector3(Random.Range(0, 1), Random.Range(0, 1), 0);
                }
                runningDirection = runningDirection.normalized * MAX_DISTANCE;
                entity.RunTowards(entity.transform.position + runningDirection);
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
        attacker = other;
        state = State.RunningAway;
    }

    public void OnSee(GameObject other)
    {
        //ignore
    }
}
