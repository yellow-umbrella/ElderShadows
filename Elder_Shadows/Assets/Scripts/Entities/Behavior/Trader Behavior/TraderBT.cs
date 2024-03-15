using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(NPC))]
[RequireComponent(typeof(Trader))]
public class TraderBT : BehaviorTree.Tree
{
    [SerializeField] private float probabilityToTeleport = 0;
    [SerializeField] private float wanderingRadius = 4f;
    private MovementController controller;
    private NPC npc;
    private Trader trader;

    protected override Node SetupTree()
    {
        controller = gameObject.GetComponent<MovementController>();
        npc = gameObject.GetComponent<NPC>();
        trader = gameObject.GetComponent<Trader>();

        Node root = new SelectorNode(new List<Node>
        {
            // if player don`t see npc
            new SequenceNode(new List<Node>
            {
                new InvisibilityCheck(npc.gameObject.transform),
                new SelectorNode(new List<Node>
                {
                    // try to teleport
                    new SequenceNode(new List<Node>
                    {
                        new TeleportCheck(probabilityToTeleport),
                        new TeleportNode(npc)
                    }),
                    // do nothing
                    new DoNothingNode()
                }),
            }),
            // if player interacting with npc
            new SequenceNode(new List<Node>{
                new PlayerInteractionCheck(npc),
                new TradeNode(trader, npc),
            }),
            new SequenceNode(new List<Node>
            {
                new UpdatePathNode(controller, wanderingRadius),
                new WalkNode(controller),
            }),
        });
        return root;
    }
}
