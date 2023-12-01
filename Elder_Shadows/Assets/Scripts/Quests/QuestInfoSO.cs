using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "Quest System/QuestInfoSO")]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id {  get; private set; }

    [Foldout("General", true)]
    [TextArea(1, 5)]
    public string displayName;
    [TextArea(5, 20)] 
    public string description;
    [TextArea(5, 20)] 
    public string objectives;

    [Foldout("Requirements", true)]
    public int levelRequirement;
    public int trustRequirement;

    [Foldout("Steps", true)]
    public GameObject[] questStepPrefabs;

    [Foldout("Rewards", true)]
    public int experience;
    public int trust;
    public int money;
    public ItemObject[] items;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
