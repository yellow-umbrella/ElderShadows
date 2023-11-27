using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityInfoSO", menuName = "Entities/EntityInfoSO")]
public class EntityInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }
    [TextArea(1, 5)]
    public string displayName;
    [TextArea(5, 20)]
    public string description;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
