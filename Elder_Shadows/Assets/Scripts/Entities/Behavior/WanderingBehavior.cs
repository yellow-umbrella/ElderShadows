using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WanderingBehavior : IIdleBehavior
{
    private BaseEntity entity;
    private Vector2 wanderingCenter;
    private float wanderingRadius;

    public WanderingBehavior(BaseEntity entity, float radius)
    {
        wanderingCenter = entity.transform.position;
        this.entity = entity;
        wanderingRadius = radius;
    }

    public bool Behave()
    {
        if (Vector2.Distance(wanderingCenter, (Vector2)entity.transform.position) > wanderingRadius)
        {
            entity.MoveTowards(wanderingCenter);
        }
        else if (entity.ReachedEndOfPath)
        {
            Vector2 newPos = (Vector2)wanderingCenter + Random.insideUnitCircle * wanderingRadius;
            entity.MoveTowards(newPos);
        }
        return true;
    }
}
