using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AoEAttackSO", menuName = "Entities/AoEAttackSO")]
public class AoEAttackSO : EntityAttackSO
{
    public float range;

    private void Awake()
    {
        attackType = AttackType.AoE;
    }
}
