using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ActiveQuestCheck : Node
{
    private QuestGiver questGiver;

    public ActiveQuestCheck(QuestGiver questGiver)
    {
        this.questGiver = questGiver;
    }

    public override NodeState Evaluate()
    {
        if (questGiver.HasActiveQuest())
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
