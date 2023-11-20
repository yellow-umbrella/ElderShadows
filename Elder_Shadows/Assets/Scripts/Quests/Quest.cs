using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public enum QuestState
    {
        REQUIREMENTS_NOT_MET = 0,
        CAN_START = 1,
        IN_PROGRESS = 2,
        CAN_FINISH = 3,
        FINISHED = 4,
    }

    public QuestInfoSO info { get; private set; }

    public QuestState state;
    private int currentStep;

    public Quest(QuestInfoSO info)
    {
        this.info = info;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentStep = 0;
    }

    public bool IsMetRequirements()
    {
        // TODO check player level and trust
        return true;
    }

    public void MoveToNextStep()
    {
        currentStep++;
    }

    public bool CurrentStepExists()
    {
        return (currentStep < info.questStepPrefabs.Length);
    }

    public void CreateCurrentQuestStep(Transform parent)
    {
        GameObject questStepPrefab = GetCurrentStepPrefab();
        if (questStepPrefab != null)
        {
            QuestStep questStep = GameObject.Instantiate(questStepPrefab, parent)
                .GetComponent<QuestStep>();
            questStep.InitQuestStep(info.id);
        }
    }

    private GameObject GetCurrentStepPrefab()
    {
        GameObject stepPrefab = null;
        if (CurrentStepExists())
        {
            stepPrefab = info.questStepPrefabs[currentStep];
        }
        return stepPrefab;
    }
}
