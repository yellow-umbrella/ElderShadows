using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Random = UnityEngine.Random;

public class WanderingBehavior : IIdleBehavior
{
    private BaseEntity entity;
    private Vector2 wanderingCenter;
    private float wanderingRadius;

    private float timeBetweenTurns = .5f;
    private float nextTurnTime = 0;

    public WanderingBehavior(BaseEntity entity, float radius)
    {
        wanderingCenter = entity.transform.position;
        this.entity = entity;
        wanderingRadius = radius;
        entity.OnReachedEndOfPath += ResetTimer;
    }

    public bool Behave()
    {
        bool tooFar = Vector2.Distance(wanderingCenter, (Vector2)entity.transform.position) > wanderingRadius;
        bool canTurn = Time.time > nextTurnTime;

        if (tooFar || (canTurn && entity.ReachedEndOfPath))
        {
            Vector2 newPos = (Vector2)wanderingCenter + Random.insideUnitCircle * wanderingRadius;
            entity.MoveTowards(newPos);
            return true;
        }

        return false;
    }

    private void ResetTimer()
    {
        nextTurnTime = Time.time + timeBetweenTurns;
    }
}
