using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttackSO", menuName = "Entities/RangedAttackSO")]
public class RangedAttackSO : EntityAttackSO
{
    public float range;
    public GameObject projectile;

    private void Awake()
    {
        attackType = AttackType.Ranged;
    }
}
