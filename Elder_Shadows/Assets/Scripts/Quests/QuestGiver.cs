using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private QuestInfoSO[] quests;

    [SerializeField] private QuestUIManager questUI;

    private QuestInfoSO activeQuest = null;

    [ContextMenu("Speak")]
    public void Speak()
    {
        if (activeQuest != null)
        {
            // TODO check if it can be finished
            bool canFinish = false;
            questUI.DisplayActiveQuest(activeQuest, canFinish, DeclineQuest, FinishQuest);
        } else
        {
            activeQuest = ChooseQuest();
            if (activeQuest != null )
            {
                Debug.Log("Offering new quest: " + activeQuest.displayName);
                questUI.OfferQuest(activeQuest, DeclineQuest, AcceptQuest);
            }
        }
    }

    private QuestInfoSO ChooseQuest()
    {
        List<QuestInfoSO> obtainableQuests = new List<QuestInfoSO>();
        foreach (QuestInfoSO quest in quests)
        {
            // TODO check if quest is obtainable
            obtainableQuests.Add(quest);
        }

        int questInd = Random.Range(0, obtainableQuests.Count);

        return quests[questInd];
    }

    public void AcceptQuest()
    {
        // TODO mark quest as IN_PROGRESS
        Debug.Log("Player accepted quest: " + activeQuest.displayName);
    }
    
    public void FinishQuest()
    {
        // TODO give rewards to player
        Debug.Log("Player finished quest: " + activeQuest.displayName);
        // TODO mark quest as FINISHED
        activeQuest = null;
    }

    public void DeclineQuest()
    {
        Debug.Log("Player declined quest: " + activeQuest.displayName);
        activeQuest = null;
    }
}
