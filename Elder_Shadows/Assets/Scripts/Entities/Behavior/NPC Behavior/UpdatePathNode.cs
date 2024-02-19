using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class UpdatePathNode : Node
{
    private MovementController controller;
    private Vector2 center;
    private float wanderingRadius;

    private float timeToStandStill = 1f;
    private float nextMovementTime = 0;

    public const string CENTER_POSITION = "center_position";

    public UpdatePathNode(MovementController controller, float wanderingRadius)
    {
        this.controller = controller;
        this.center = controller.transform.position;
        this.wanderingRadius = wanderingRadius;
    }

    public override NodeState Evaluate()
    {
        // if other nodes changed center
        object newCenter = GetData(CENTER_POSITION);
        if (newCenter != null && center != (Vector2)newCenter)
        {
            center = (Vector2)newCenter;
            SetNewPath();
        }
        
        // need to find new path
        if (controller.ReachedEndOfPath && !controller.IsGeneratingPath)
        {
            SetNewPath();
        }

        // there is existing path and obj can move on it
        if (Time.time >= nextMovementTime && !controller.IsGeneratingPath)
        {
            return NodeState.SUCCESS;
        }

        // in process of findig path
        return NodeState.RUNNING;
    }
    
    private void SetNewPath()
    {
        controller.SetPath(ChooseNewPosition());
        ResetTime();
    }

    // position inside wandering circle
    private Vector2 ChooseNewPosition()
    {
        return center + Random.insideUnitCircle * wanderingRadius;
    }

    private void ResetTime()
    {
        nextMovementTime = Time.time + timeToStandStill;
    }
}
