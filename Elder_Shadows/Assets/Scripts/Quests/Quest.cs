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
    private QuestStepState[] questStepStates;

    public Quest(QuestInfoSO info)
    {
        this.info = info;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentStep = 0;
        this.questStepStates = new QuestStepState[info.questStepPrefabs.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfoSO info, QuestState state, int currentStep, QuestStepState[] questStepStates)
    {
        this.info = info;
        this.state = state;
        this.currentStep = currentStep;
        this.questStepStates = questStepStates;

        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning("Saved Data out of sync with QuestInfo");
        }
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
            questStep.InitQuestStep(info.id, currentStep, 
                questStepStates[currentStep].state);
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

    public QuestData GetQuestData()
    {
        return new QuestData(state, currentStep, questStepStates);
    }

    public void StoreQuestStepState(QuestStepState state, int index)
    {
        if (index < questStepStates.Length)
        {
            questStepStates[index].state = state.state;
        }
    }
}
