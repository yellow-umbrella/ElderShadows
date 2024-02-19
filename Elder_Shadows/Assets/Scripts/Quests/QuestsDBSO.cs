using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestsDB", menuName = "Quest System/QuestsDB")]
public class QuestsDBSO:ScriptableObject
{
    public QuestInfoSO[] quests;
}
