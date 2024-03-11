using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityAttackSO : ScriptableObject
{
    public IAttackable.DamageType dmgType;
    public float dmgMultiplier;
    public List<AttackDebuff> debufs;
    public AttackType attackType;

    [Serializable]
    public struct AttackDebuff
    {
        //public Buff debuff;
        public float chance;
    }

    public enum AttackType
    {
        Targeted = 0,
        AoE = 1,
        Ranged = 2,
        Summon = 3,
    }
}
