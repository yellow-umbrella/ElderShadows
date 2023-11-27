using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;
    private int stepIndex;

    public void InitQuestStep(string questId, int stepIndex, string questStepState)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        if (questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
        QuestManager.instance.onQuestDecline += QuestManager_onQuestDecline;
    }

    private void QuestManager_onQuestDecline(string questId)
    {
        if (questId == this.questId)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        QuestManager.instance.onQuestDecline -= QuestManager_onQuestDecline;
    }

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;

            QuestManager.instance.AdvanceQuest(questId);

            Destroy(this.gameObject);
        }
    }

    protected void ChangeState(string newState)
    {
        QuestManager.instance.ChangeQuestStepState(questId, stepIndex, 
            new QuestStepState(newState));
    }

    protected abstract void SetQuestStepState(string state);
}
