using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ActiveQuestNode : Node
{
    private QuestGiver questGiver;

    public ActiveQuestNode(QuestGiver questGiver)
    {
        this.questGiver = questGiver;
    }

    public override NodeState Evaluate()
    {
        if ((Node)parent.GetData(PREV_ACTION) != this)
        {
            questGiver.ShowActiveQuest();
        }
        state = NodeState.RUNNING;
        return state;
    }
}
