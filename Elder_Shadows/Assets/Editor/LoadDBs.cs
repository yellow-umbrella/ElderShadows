using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;
using System;

public class LoadDBs
{
    private static string questCSVPath = "/Editor/CSVs/QuestsDB.tsv";
    private static string questDBPath = "Assets/Scripts/Quests/QuestsDB.asset";
    
    private static string itemsCSVPath = "/Editor/CSVs/ItemsDB.tsv";
    private static string itemsDBPath = "Assets/Scripts/Inventory/Items/ItemDatabase.asset";

    [MenuItem("Utilities/Generate Quests")]
    public static void GenerateQuests()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + questCSVPath);
        bool isNewQuest;

        QuestsDBSO questDB = AssetDatabase.LoadAssetAtPath<QuestsDBSO>(questDBPath);
        List<QuestInfoSO> quests = new List<QuestInfoSO>();

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
                    UnityEngine.Object.DestroyImmediate(questStepObj);
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

            quests.Add(quest);
        }
        questDB.quests = quests.ToArray();
        EditorUtility.SetDirty(questDB);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Items")]
    public static void GenerateItems()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + itemsCSVPath);
        bool isNewItem;

        ItemDatabaseObject itemDB = AssetDatabase.LoadAssetAtPath<ItemDatabaseObject>(itemsDBPath);
        List<ItemObject> items = new List<ItemObject>(itemDB.ItemObjects);

        for (int i = 1; i < allLines.Length; i++)
        {
            string[] splitData = allLines[i].Split('\t');
            string itemSOPath = $"Assets/Scripts/Inventory/Items/{splitData[1]}.asset";
            ItemObject item = AssetDatabase.LoadAssetAtPath<ItemObject>(itemSOPath);

            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemObject>();
                isNewItem = true;
            }
            else
            {
                isNewItem = false;
            }

            item.description = splitData[2];
            item.type = (ItemType)Enum.Parse(typeof(ItemType), splitData[4]);
            item.data.Id = int.Parse(splitData[0]);
            item.data.Name = splitData[1];

            if (splitData[3] != "")
            {
                string[] stats = splitData[3].Split(",");
                List<ItemBuff> buffs = new List<ItemBuff>();
                foreach (string stat in stats)
                {
                    string[] statParts = stat.Split(":");
                    ItemBuff buff = new ItemBuff(int.Parse(statParts[1]), int.Parse(statParts[2]));
                    buff.attribute = (Attributes)Enum.Parse(typeof(Attributes), statParts[0]);
                    buffs.Add(buff);
                }

                item.data.buffs = buffs.ToArray();
            }

            if (isNewItem)
            {
                AssetDatabase.CreateAsset(item, itemSOPath);
                items.Add(item);
            }
        }

        itemDB.ItemObjects = items.ToArray();
        EditorUtility.SetDirty(itemDB);
        AssetDatabase.SaveAssets();
    }
}
