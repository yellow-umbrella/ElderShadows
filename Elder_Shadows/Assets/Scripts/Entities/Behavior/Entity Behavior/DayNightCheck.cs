using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class DayNightCheck : Node
{
    private BaseEntity entity;

    public DayNightCheck(BaseEntity entity)
    {
        this.entity = entity;
    }

    public override NodeState Evaluate()
    {
        if (entity.Info.entityType == EntityInfoSO.EntityType.Day && DaycycleManager.instance.IsNight 
            || entity.Info.entityType == EntityInfoSO.EntityType.Night && !DaycycleManager.instance.IsNight)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
