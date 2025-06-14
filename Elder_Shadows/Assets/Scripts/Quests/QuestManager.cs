using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using MyBox;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField] private QuestsDBSO allQuestInfos;

    private Dictionary<string, Quest> quests;

    private const string PATH = "/quests/";

    private void Awake()
    {
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

    private void Start()
    {
        foreach (Quest quest in quests.Values)
        {
            if (quest.state == Quest.QuestState.IN_PROGRESS)
            {
                quest.CreateCurrentQuestStep(this.transform);
            }
            onQuestStateChange?.Invoke(quest);
        }
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
        foreach (QuestInfoSO questInfo in allQuestInfos.quests)
        {
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
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
    
    public event Action<string, int, QuestStepState> onQuestStepStateChange;
    public void ChangeQuestStepState(string questId, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(questId);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(questId, quest.state);
        onQuestStepStateChange?.Invoke(questId, stepIndex, questStepState);
    }

    public event Action<string> onTryFinishQuest;
    public event Action<string> onFinishQuest;
    public bool FinishQuest(string id)
    {
        Debug.Log("Trying to finish quest " + id);
        onTryFinishQuest?.Invoke(id);
        if (GetQuestById(id).state != Quest.QuestState.CAN_FINISH)
        {
            return false;
        }
        ChangeQuestState(id, Quest.QuestState.FINISHED);
        // give rewards to player
        QuestInfoSO info = GetQuestById(id).info;
        foreach (var item in info.items)
        {
            CharacterController.instance.inventory.AddItem(new Item(item), 1);
        }
        CharacterController.instance.dataManager.AddExperience(info.experience);
        CharacterController.instance.dataManager.AddTrust(info.trust);
        CharacterController.instance.dataManager.AddMoney(info.money);
        onFinishQuest?.Invoke(id);
        return true;
    }

    public event Action<string> onQuestDecline;
    public void DeclineQuest(string id)
    {
        quests[id] = new Quest(GetQuestById(id).info);
        onQuestDecline?.Invoke(id);
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in quests.Values)
        {
            SaveQuest(quest);
        }
    }

    private void OnDestroy()
    {
        foreach (Quest quest in quests.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            string path = Application.persistentDataPath + PATH;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(path + quest.info.id + ".json", serializedData);
            //Debug.Log("Saved quest with id: " + quest.info.id + ": " + serializedData);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try
        {
            string path = Application.persistentDataPath + PATH + questInfo.id + ".json";
            if (File.Exists(path))
            {
                string serializedData = File.ReadAllText(path);
                //Debug.Log("Loading quest with id: " + questInfo.id + ": " + serializedData);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            } else
            {
                quest = new Quest(questInfo);
            }
        } catch (Exception e)
        {
            Debug.LogError("Failed to load quest with id " + questInfo.id + ": " + e);
        }

        return quest;
    }

    [ButtonMethod]
    public void ClearSavedQuests()
    {
        DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath + PATH);
        dataDir.Delete(true);
    }
}
