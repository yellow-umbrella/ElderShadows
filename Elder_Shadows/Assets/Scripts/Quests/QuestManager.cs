using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField] private List<QuestInfoSO> allQuestInfos;

    private Dictionary<string, Quest> quests;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        quests = CreateQuestsDictionary();
    }

    private void Update()
    {
        foreach (Quest quest in quests.Values)
        {
            if (quest.state == Quest.QuestState.REQUIREMENTS_NOT_MET && quest.IsMetRequirements())
            {
                ChangeQuestState(quest.info.id, Quest.QuestState.CAN_START);
            }
        }
    }

    private Dictionary<string, Quest> CreateQuestsDictionary()
    {
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuestInfos)
        {
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    public Quest GetQuestById(string id)
    {
        Quest quest = quests[id];
        if (quest == null)
        {
            Debug.LogError("ID of quest not found in Quests Dictionary: " + id);
        }
        return quest;
    }

    public event Action<string> onStartQuest;
    public void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.CreateCurrentQuestStep(this.transform);
        ChangeQuestState(id, Quest.QuestState.IN_PROGRESS);

        Debug.Log("QuestManager started quest: " + id);

        onStartQuest?.Invoke(id);
    }

    public event Action<string> onAdvanceQuest;
    public void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();
        if (quest.CurrentStepExists())
        {
            quest.CreateCurrentQuestStep(this.transform);
        } else
        {
            ChangeQuestState(id, Quest.QuestState.CAN_FINISH);
        }

        onAdvanceQuest?.Invoke(id);
    }

    public event Action<Quest> onQuestStateChange;
    public void ChangeQuestState(string id, Quest.QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        onQuestStateChange?.Invoke(quest);
    }

    public event Action<string> onFinishQuest;
    public void FinishQuest(string id)
    {
        ChangeQuestState(id, Quest.QuestState.FINISHED);
        onFinishQuest?.Invoke(id);
    }

    public event Action<string> onQuestDecline;
    public void DeclineQuest(string id)
    {
        quests[id] = new Quest(GetQuestById(id).info);
        onQuestDecline?.Invoke(id);
    }
}
