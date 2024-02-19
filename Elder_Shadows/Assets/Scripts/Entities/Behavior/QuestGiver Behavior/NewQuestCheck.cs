using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class NewQuestCheck : Node
{
    private QuestGiver questGiver;

    public NewQuestCheck(QuestGiver questGiver)
    {
        this.questGiver = questGiver;
    }

    public override NodeState Evaluate()
    {
        if (questGiver.CanOfferQuest())
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
