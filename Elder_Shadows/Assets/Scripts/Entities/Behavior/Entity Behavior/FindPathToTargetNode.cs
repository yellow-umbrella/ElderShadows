using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPathToTargetNode : Node
{
    private MovementController controller;

    private float genCooldown = 1f;
    private bool canGenerate = true;
    private float offset = .5f;

    private GameObject target = null;

    public FindPathToTargetNode(MovementController controller)
    {
        this.controller = controller;
    }

    public override NodeState Evaluate()
    {
        GameObject newTarget = (GameObject)GetData(AttackTargetNode.ATTACK_TARGET);

        // already close enough
        if (Vector2.Distance(controller.transform.position, newTarget.transform.position) < offset)
        {
            state = NodeState.FAILURE;
            return state;
        }

        // need to find new path
        if (!controller.IsGeneratingPath && (controller.ReachedEndOfPath || canGenerate || target != newTarget))
        {
            target = newTarget;
            controller.SetPath(target.transform.position);
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