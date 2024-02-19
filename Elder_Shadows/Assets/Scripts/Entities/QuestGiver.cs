using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestGiver : NPC
{
    public event Action<Quest.QuestState> onActiveQuestStateChange;

    [SerializeField] private List<QuestInfoSO> quests;
    [SerializeField] private QuestUIManager questUI;

    private Quest activeQuest = null;
    private Quest newQuest = null;

    private void Start()
    {
        QuestManager.instance.onQuestStateChange += QuestManager_onQuestStateChange; ;       
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
    private void OnDestroy()
    {
        QuestManager.instance.onQuestStateChange -= QuestManager_onQuestStateChange; ;
    }

    private void QuestManager_onQuestStateChange(Quest quest)
    {
        if (activeQuest == quest)
        {
            onActiveQuestStateChange?.Invoke(quest.state);
        }
    }

    private void FixedUpdate()
    {
        if (!HasActiveQuest() && CanOfferQuest())
        {
            onActiveQuestStateChange(Quest.QuestState.CAN_START);
        }
    }

    public bool HasActiveQuest()
    {
        return activeQuest != null;
    }

    public void ShowActiveQuest()
    {
        questUI.DisplayActiveQuest(activeQuest.info, AbortQuest, FinishInteraction);
    }

    public bool CanOfferQuest()
    {
        if (newQuest == null)
        {
            newQuest = ChooseQuest();
        } 
        return newQuest != null;
    }
    
    public void OfferQuest()
    {
        questUI.OfferQuest(newQuest.info, DeclineQuest, AcceptQuest, DeclineQuest);
        Debug.Log("Offering new quest: " + newQuest.info.displayName);
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
    
    public bool FinishQuest()
    {
        if (QuestManager.instance.FinishQuest(activeQuest.info.id))
        {
            Debug.Log("Player finished quest: " + activeQuest.info.displayName);
            activeQuest = null;
            onActiveQuestStateChange(Quest.QuestState.REQUIREMENTS_NOT_MET);
            return true;
        }
        return false;
    }

    private void AcceptQuest()
    {
        activeQuest = QuestManager.instance.GetQuestById(newQuest.info.id);
        newQuest = null;
        onActiveQuestStateChange?.Invoke(activeQuest.state);
        QuestManager.instance.StartQuest(activeQuest.info.id);
        Debug.Log("Player accepted quest: " + activeQuest.info.displayName);
        FinishInteraction();
    }
    
    private void AbortQuest()
    {
        Debug.Log("Player declined quest: " + activeQuest.info.displayName);
        QuestManager.instance.DeclineQuest(activeQuest.info.id);
        activeQuest = null;
        onActiveQuestStateChange(Quest.QuestState.REQUIREMENTS_NOT_MET);
        FinishInteraction();
    }

    private void DeclineQuest()
    {
        Debug.Log("Player aborted quest: " + newQuest.info.displayName);
        activeQuest = null;
        newQuest = null;
        FinishInteraction();
    }

    public void AddNewQuest(QuestInfoSO quest)
    {
        quests.Add(quest);
    }
}
