using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class NpcBT : BehaviorTree.Tree
{
    [SerializeField] private float probabilityToTeleport = 0;
    [SerializeField] private float wanderingRadius = 4f;
    private MovementController controller;
    private NPC npc;

    protected override Node SetupTree()
    {
        controller = gameObject.GetComponent<MovementController>();
        npc = gameObject.GetComponent<NPC>();
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
                new ShowDialogNode(npc)
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
