using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class QuestGiverBT : BehaviorTree.Tree
{
    [SerializeField] private MovementController controller;
    [SerializeField] private QuestGiver questGiver;
    [SerializeField] private float probabilityToTeleport = 0;
    [SerializeField] private float wanderingRadius = 4f;

    protected override Node SetupTree()
    {
        Node root = new SelectorNode(new List<Node>
        {
            // if player don`t see npc
            new SequenceNode(new List<Node>
            {
                new InvisibilityCheck(questGiver.gameObject.transform),
                new SelectorNode(new List<Node>
                {
                    // try to teleport
                    new SequenceNode(new List<Node>
                    {
                        new TeleportCheck(probabilityToTeleport),
                        new TeleportNode(questGiver)
                    }),
                    // do nothing
                    new DoNothingNode()
                }),
            }),
            // if player interacting with npc
            new SequenceNode(new List<Node>{
                new PlayerInteractionCheck(questGiver),
                new SelectorNode(new List<Node>
                {
                    // show active quest
                    new SequenceNode(new List<Node>
                    {
                        new ActiveQuestCheck(questGiver),
                        new ActiveQuestNode(questGiver)
                    }),
                    // propose new quest
                    new SequenceNode(new List<Node>
                    {
                        new NewQuestCheck(questGiver),
                        new NewQuestNode(questGiver)
                    }),
                    // just speak
                    new ShowDialogNode(questGiver),
                }),
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
