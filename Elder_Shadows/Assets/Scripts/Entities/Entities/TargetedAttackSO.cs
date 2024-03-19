using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetedAttackSO", menuName = "Entities/TargetedAttackSO")]
public class TargetedAttackSO : EntityAttackSO
{
    public float range;

    private void Awake()
    {
        attackType = AttackType.Targeted;
    }
}
