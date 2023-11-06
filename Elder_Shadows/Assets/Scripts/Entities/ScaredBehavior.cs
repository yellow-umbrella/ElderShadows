using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ScaredBehavior : IAttackBehavior
{
    private enum State
    {
        Idle = 0,
        RunningAway = 1,
    }
    private State state;
    private BaseEntity entity;
    private GameObject intruder;
    private const float MAX_DISTANCE = 10;

    public ScaredBehavior(BaseEntity entity)
    {
        this.entity = entity;
        state = State.Idle;
    }

    public bool Behave()
    {
        if (state == State.RunningAway)
        {
            if (intruder != null && entity.CanSee(intruder))
            {
                Vector3 runningDirection = entity.transform.position - intruder.transform.position;
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
        OnSee(other);
    }

    public void OnSee(GameObject other)
    {
        float distIntruder = float.PositiveInfinity;
        float distOther = Vector3.Distance(other.transform.position, entity.transform.position);
        if (intruder != null)
        {
            distIntruder = Vector3.Distance(intruder.transform.position, entity.transform.position);
        }

        if (distOther <= distIntruder)
        {
            intruder = other;
            state = State.RunningAway;
        }
    }
}
