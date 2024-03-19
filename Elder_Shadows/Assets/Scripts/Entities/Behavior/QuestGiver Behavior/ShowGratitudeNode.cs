using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ShowGratitudeNode : Node
{
    private QuestGiver questGiver;
    private float timeToShow = 5f;
    private float timeToHide = 0f;

    public ShowGratitudeNode(QuestGiver questGiver)
    {
        this.questGiver = questGiver;
    }

    public const string GRATITUDE = "gratitude";

    public override NodeState Evaluate()
    {
        object obj = GetData(GRATITUDE);
        if (obj != null && (bool)obj)
        {
            if ((Node)parent.GetData(PREV_ACTION_CHILD) != this 
                && questGiver.ShowGratitudeDialog())
            {
                timeToHide = Time.time + timeToShow;
                state = NodeState.RUNNING;
                return state;
            }
            if (Time.time >= timeToHide)
            {
                questGiver.StopTalking();
                GetRoot().SetData(GRATITUDE, false);
                state = NodeState.SUCCESS; 
                return state;
            }
            state = NodeState.RUNNING;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
