using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonAttackSO", menuName = "Entities/SammonAttackSO")]
public class SummonAttackSO : EntityAttackSO
{
    public List<BaseEntity> entities;
    public int amount;

    private void Awake()
    {
        attackType = AttackType.Summon;
    }
}
