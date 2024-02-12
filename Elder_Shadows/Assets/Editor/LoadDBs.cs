using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;

public class LoadDBs
{
    private static string questCSVPath = "/Editor/CSVs/QuestsDB.tsv";

    [MenuItem("Utilities/Generate Quests")]
    public static void GenerateQuests()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + questCSVPath);
        bool isNewQuest;

        string dbPath = "Assets/Scripts/Quests/QuestsDB.asset";
        QuestsDBSO questDB = AssetDatabase.LoadAssetAtPath<QuestsDBSO>(dbPath);
        questDB.quests = new List<QuestInfoSO>();

        for (int i = 1; i < allLines.Length; i++)
        {
            string[] splitData = allLines[i].Split('\t');

            string questSOPath = $"Assets/Scripts/Quests/QuestInfoSOs/{splitData[0]}.asset";
            QuestInfoSO quest = AssetDatabase.LoadAssetAtPath<QuestInfoSO>(questSOPath);
            if (quest == null )
            {
                quest = ScriptableObject.CreateInstance<QuestInfoSO>();
                isNewQuest = true;
            } else
            {
                isNewQuest = false;
            }

            quest.displayName = splitData[1];
            quest.description = splitData[2];
            quest.objectives = splitData[3];
            quest.experience = int.Parse(splitData[4]);
            quest.trust = int.Parse(splitData[5]);
            quest.money = int.Parse(splitData[6]);
            // skiping reward_items column splitData[7]
            quest.levelRequirement = int.Parse(splitData[8]);
            quest.trustRequirement = int.Parse(splitData[9]);
            // skiping Requirements_quests column splitData[10]
            // skiping Questgiver column splitData[11]
            quest.questStepPrefabs = new List<GameObject>();
            string[] steps = splitData[12].Split(',');
            // creating and adding quest steps to quest 
            foreach (string step in steps)
            {
                // parts[0] - type of step; parts[1] - step name
                string[] parts = step.Split(":");
                string stepPrefabPath = $"Assets/Prefabs/Quests/{parts[1]}.prefab";
                // creating new prefab if it doesn't exist
                if (!File.Exists(stepPrefabPath))
                {
                    GameObject questStepObj = new GameObject();
                    switch (parts[0])
                    {
                        case "Gathering":
                            questStepObj = new GameObject(parts[1], typeof(CollectQuestStep));
                            break;
                        case "Killing":
                            questStepObj = new GameObject(parts[1], typeof(KillQuestStep));
                            break;
                    }
                    PrefabUtility.SaveAsPrefabAsset(questStepObj, stepPrefabPath);
                    Object.DestroyImmediate(questStepObj);
                }
                // adding prefab to list of steps
                GameObject questStep = AssetDatabase.LoadAssetAtPath<GameObject>(stepPrefabPath);
                quest.questStepPrefabs.Add(questStep);
            }
            
            if (isNewQuest)
            {
                AssetDatabase.CreateAsset(quest, questSOPath);
                quest.UpdateID();
            }

            questDB.quests.Add(quest);
        }
        AssetDatabase.SaveAssets();
    }
}
