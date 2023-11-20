using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private QuestInfoSO[] quests;

    [SerializeField] private QuestUIManager questUI;

    private Quest activeQuest = null;

    private void OnEnable()
    {
        QuestManager.instance.onQuestStateChange += QuestManager_onQuestStateChange; ;       
    }

    private void OnDisable()
    {
        QuestManager.instance.onQuestStateChange -= QuestManager_onQuestStateChange; ;
    }

    public event Action<Quest.QuestState> onActiveQuestStateChange;
    private void QuestManager_onQuestStateChange(Quest quest)
    {
        if (activeQuest == quest)
        {
            onActiveQuestStateChange?.Invoke(quest.state);
        }
    }

    private void Start()
    {
        foreach(QuestInfoSO questInfo in quests)
        {
            Quest quest = QuestManager.instance.GetQuestById(questInfo.id);
            if (quest != null && (quest.state == Quest.QuestState.IN_PROGRESS 
                || quest.state == Quest.QuestState.CAN_FINISH))
            {
                activeQuest = quest;
                onActiveQuestStateChange(activeQuest.state);
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (activeQuest == null && ChooseQuest() != null)
        {
            onActiveQuestStateChange(Quest.QuestState.CAN_START);
        }
    }

    [ContextMenu("Speak")]
    public void Speak()
    {
        if (activeQuest != null)
        {
            bool canFinish = (activeQuest.state == Quest.QuestState.CAN_FINISH);
            questUI.DisplayActiveQuest(activeQuest.info, canFinish, DeclineQuest, FinishQuest);
        } else
        {
            activeQuest = ChooseQuest();
            if (activeQuest != null )
            {
                onActiveQuestStateChange?.Invoke(activeQuest.state);
                Debug.Log("Offering new quest: " + activeQuest.info.displayName);
                questUI.OfferQuest(activeQuest.info, DeclineQuest, AcceptQuest);
            }
        }
    }

    private Quest ChooseQuest()
    {
        List<Quest> obtainableQuests = new List<Quest>();
        foreach (QuestInfoSO questInfo in quests)
        {
            Quest quest = QuestManager.instance.GetQuestById(questInfo.id);
            if (quest != null && quest.state == Quest.QuestState.CAN_START)
            {
                obtainableQuests.Add(quest);
            }
        }

        if (obtainableQuests.Count > 0)
        {
            int questInd = Random.Range(0, obtainableQuests.Count);
            return obtainableQuests[questInd];
        }

        return null;
    }

    public void AcceptQuest()
    {
        QuestManager.instance.StartQuest(activeQuest.info.id);
        Debug.Log("Player accepted quest: " + activeQuest.info.displayName);
    }
    
    public void FinishQuest()
    {
        // TODO give rewards to player
        Debug.Log("Player finished quest: " + activeQuest.info.displayName);
        QuestManager.instance.FinishQuest(activeQuest.info.id);
        activeQuest = null;
    }

    public void DeclineQuest()
    {
        QuestManager.instance.DeclineQuest(activeQuest.info.id);
        Debug.Log("Player declined quest: " + activeQuest.info.displayName);
        activeQuest = null;
        onActiveQuestStateChange(Quest.QuestState.REQUIREMENTS_NOT_MET);
    }
}
