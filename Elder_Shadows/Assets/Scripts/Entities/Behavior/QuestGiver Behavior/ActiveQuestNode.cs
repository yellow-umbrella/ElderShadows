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
        GetRoot().SetData(ShowGratitudeNode.GRATITUDE, false);

        if ((Node)parent.GetData(PREV_ACTION) != this)
        {
            if (questGiver.FinishQuest())
            {
                GetRoot().SetData(ShowGratitudeNode.GRATITUDE, true);
                state = NodeState.FAILURE;
                return state;
            }
            questGiver.ShowActiveQuest();
        }

        if (!questGiver.IsInteracting())
        {
            state = NodeState.SUCCESS; 
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}
