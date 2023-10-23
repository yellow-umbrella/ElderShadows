using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RelaxedBehavior : IBehavior
{
    private enum State
    {
        Wandering = 0,
        RunningAway = 1,
    }
    private State state = State.Wandering;
    private BaseEntity entity;
    private GameObject attacker;
    private const float MAX_DISTANCE = 100;

    public void Behave()
    {
        switch (state)
        {
            case State.Wandering:
                entity.WanderAround();
                break;
            case State.RunningAway:
                if (attacker != null && entity.CanSee(attacker))
                {
                    Vector3 runningDirection = entity.transform.position - attacker.transform.position;
                    if (runningDirection.ToVector2() == Vector2.zero)
                    {
                        runningDirection = new Vector3(Random.Range(0, 1), Random.Range(0, 1), 0);
                    }
                    runningDirection = runningDirection.normalized * MAX_DISTANCE;
                    entity.RunTowards(entity.transform.position + runningDirection);
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
        attacker = other;
        state = State.RunningAway;
    }

    public void OnSee(GameObject other)
    {
        //ignore
    }
}
