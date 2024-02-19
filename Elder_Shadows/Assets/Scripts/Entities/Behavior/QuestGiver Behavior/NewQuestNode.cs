using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class NewQuestNode : Node
{
    private QuestGiver questGiver;

    public NewQuestNode(QuestGiver questGiver)
    {
        this.questGiver = questGiver;
    }

    public override NodeState Evaluate()
    {
        if ((Node)parent.GetData(PREV_ACTION) != this)
        {
            questGiver.OfferQuest();
        }
        state = NodeState.RUNNING;
        return state;
    }
}
