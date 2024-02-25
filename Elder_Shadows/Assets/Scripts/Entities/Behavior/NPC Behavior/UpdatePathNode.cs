using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class UpdatePathNode : Node
{
    private MovementController controller;
    private Vector2 center;
    private float wanderingRadius;

    private float movementCooldown = 1f;
    private bool canMove = true;

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
        if (canMove && !controller.IsGeneratingPath)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        // in process of findig path
        state = NodeState.RUNNING;
        return state;
    }
    
    private void SetNewPath()
    {
        controller.SetPath(ChooseNewPosition());
        controller.StartCoroutine(MovementCooldown());
    }

    // position inside wandering circle
    private Vector2 ChooseNewPosition()
    {
        return center + Random.insideUnitCircle * wanderingRadius;
    }

    private IEnumerator MovementCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(movementCooldown);
        canMove = true;
    }
}
