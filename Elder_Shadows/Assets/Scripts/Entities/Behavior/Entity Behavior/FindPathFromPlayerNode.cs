using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPathFromPlayerNode : Node
{
    private MovementController controller;
    private float dist = 4f;
    private float genCooldown = 2f;
    private bool canGenerate = true;

    public FindPathFromPlayerNode(MovementController movementController)
    {
        controller = movementController;
    }

    public override NodeState Evaluate()
    {
        // need to find new path
        if (!controller.IsGeneratingPath && (controller.ReachedEndOfPath || canGenerate))
        {
            GameObject player = CharacterController.instance.gameObject;
            Vector3 newDirection = controller.transform.position - player.transform.position;
            Vector2 pos = controller.transform.position + newDirection.normalized * dist;
            controller.SetPath(pos);
            controller.StartCoroutine(GenCooldown());
        }

        // there is existing path and obj can move on it
        if (!controller.IsGeneratingPath)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        // in process of findig path
        state = NodeState.RUNNING;
        return state;
    }

    private IEnumerator GenCooldown()
    {
        canGenerate = false;
        parent.SetData(WalkNode.FINISH_WALK, false);
        yield return new WaitForSeconds(genCooldown);
        canGenerate = true;
        parent.SetData(WalkNode.FINISH_WALK, true);
    }
}
