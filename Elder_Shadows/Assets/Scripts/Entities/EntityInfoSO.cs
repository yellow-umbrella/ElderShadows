using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseEntity;

[CreateAssetMenu(fileName = "EntityInfoSO", menuName = "Entities/EntityInfoSO")]
public class EntityInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }
    [TextArea(1, 5)]
    public string displayName;
    [TextArea(5, 20)]
    public string description;
    public List<EntityAttackSO> attacks;
    public List<EntityAttackSO> specialAttacks;

    public float health;
    public float speed;

    public float physDmg;
    public float physRes;
    
    public float magDmg;
    public float magRes;

    public float attackCooldown;
    public float specialAttackCooldown;
    public float specialAttackChance;
    public float seeingRange;

    public Behavior reactionToPlayer;
    public Behavior reactionToTrustedPlayer;
    public int trustRequired;
    public LayerMask aggressionTargets = new LayerMask();
    public LayerMask targets = new LayerMask();
    public LayerMask enemies = new LayerMask();
    public LayerMask scaryEnemies = new LayerMask();

    public int trustForKill;
    public int expForKill;

    public List<Modifier> possibleModifiers;
    public List<Drop> dropItems;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
