using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class EntityBT : BehaviorTree.Tree
{
    [SerializeField] private MovementController controller;
    [SerializeField] private BaseEntity entity;
    [SerializeField] private float wanderingRadius = 4f;
    [SerializeField] private float attackCooldown = 1f;

    protected override Node SetupTree()
    {
        Node root = new SelectorNode(new List<Node>
        {
            // attack existing target
            new SequenceNode(new List<Node>
            {
                new TargetInAttackRangeCheck(entity),
                new AttackTargetNode(entity, attackCooldown)
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
}
