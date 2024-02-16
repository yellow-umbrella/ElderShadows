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
    private float chanceToOfferQuest = .5f;

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
        if (activeQuest == null && ChooseQuest() != null)
        {
            onActiveQuestStateChange(Quest.QuestState.CAN_START);
        }
    }

    public bool OfferQuest()
    {
        if (activeQuest != null)
        {
            bool canFinish = (activeQuest.state == Quest.QuestState.CAN_FINISH);
            questUI.DisplayActiveQuest(activeQuest.info, true, DeclineQuest, FinishQuest);
            return true;
        } 
        Quest quest = ChooseQuest();
        if (quest != null)
        {
            activeQuest = quest;
            onActiveQuestStateChange?.Invoke(activeQuest.state);
            Debug.Log("Offering new quest: " + activeQuest.info.displayName);
            questUI.OfferQuest(activeQuest.info, DeclineQuest, AcceptQuest);
            return true;
        }
        return false;
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

    private void AcceptQuest()
    {
        QuestManager.instance.StartQuest(activeQuest.info.id);
        questUI.HideQuestUI();
        Debug.Log("Player accepted quest: " + activeQuest.info.displayName);
    }
    
    private void FinishQuest()
    {
        if (QuestManager.instance.FinishQuest(activeQuest.info.id))
        {
            Debug.Log("Player finished quest: " + activeQuest.info.displayName);
            activeQuest = null;
            questUI.HideQuestUI();
        }
    }

    private void DeclineQuest()
    {
        QuestManager.instance.DeclineQuest(activeQuest.info.id);
        Debug.Log("Player declined quest: " + activeQuest.info.displayName);
        activeQuest = null;
        onActiveQuestStateChange(Quest.QuestState.REQUIREMENTS_NOT_MET);
        questUI.HideQuestUI();
    }

    public override void Interact()
    {
        if (!OfferQuest())
        {
            base.Interact();
        }
    }

    public void AddNewQuest(QuestInfoSO quest)
    {
        quests.Add(quest);
    }
}
