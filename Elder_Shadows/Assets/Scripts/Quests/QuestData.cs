using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public Quest.QuestState state;
    public int questStepIndex;

    public QuestData(Quest.QuestState state, int questStepIndex)
    {
        this.state = state;
        this.questStepIndex = questStepIndex;
    }
}
