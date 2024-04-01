using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(BaseEntity))]
public class EntityBT : BehaviorTree.Tree
{
    [SerializeField] private float wanderingRadius = 4f;

    private MovementController controller;
    private BaseEntity entity;

    protected override Node SetupTree()
    {
        controller = gameObject.GetComponent<MovementController>();
        entity = gameObject.GetComponent<BaseEntity>();

        Node root = new SelectorNode(new List<Node>
        {
            // delete if too far from player
            new SequenceNode(new List<Node>
            {
                new TooFarCheck(entity.MaxDistanceFromPlayer, entity.transform),
                new DeleteEntityNode(entity),
            }),
            // run away if not its activity time 
            new SequenceNode(new List<Node>
            {
                new DayNightCheck(entity),
                new FindPathFromPlayerNode(controller),
                new WalkNode(controller)
            }),
            // attack existing target
            new SequenceNode(new List<Node>
            {
                new SelectorNode(new List<Node>
                {
                    new ChooseSpecialAttackNode(entity),
                    new ChooseNormalAttackNode(entity)
                }),
                new TargetInAttackRangeCheck(entity),
                new AttackTargetNode(entity)
            }),
            // find new target and go to it
            new SequenceNode(new List<Node>
            {
                new TargetInSeeingRangeCheck(entity),
                new FindPathToTargetNode(controller),
                new WalkNode(controller)
            }),
            // run away from enemy
            new SequenceNode(new List<Node>
            {
                new EnemyInSeeingRangeCheck(entity),
                new FindPathFromEnemyNode(controller),
                new WalkNode(controller)
            }),
            // wander around
            new SequenceNode(new List<Node>
            {
                new UpdatePathNode(controller, wanderingRadius),
                new WalkNode(controller),
            }),
        });
        return root;
    }

    public void StartFocusedAttack(GameObject target, float time)
    {
        StartCoroutine(FocusedAttackTimer(target, time));
    }

    IEnumerator FocusedAttackTimer(GameObject target, float time)
    {
        yield return new WaitForFixedUpdate();
        root.SetData(AttackTargetNode.ATTACK_TARGET, target);
        root.SetData(TargetInSeeingRangeCheck.FOCUSED_ATTACK, true);
        yield return new WaitForSeconds(time);
        root.SetData(TargetInSeeingRangeCheck.FOCUSED_ATTACK, false);
    }
}
