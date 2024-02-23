using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityAttackSO", menuName = "Entities/EntityAttackSO")]
public class EntityAttackSO : ScriptableObject
{
    public IAttackable.DamageType dmgType;
    public float dmgMultiplier;
    public List<AttackDebuff> debufs;
    public AttackType attackType;
    public float range;

    [Serializable]
    public struct AttackDebuff
    {
        //public Buff debuff;
        public float chance;
    }

    public enum AttackType
    {
        OneTarget = 0,
        AoE = 1,
        Ranged = 2,
    }
}
