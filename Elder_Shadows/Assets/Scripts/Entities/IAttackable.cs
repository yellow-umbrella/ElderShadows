using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public enum State
    {
        Alive = 0, 
        Dead = 1,
    }

    public enum DamageType
    {
        Physical,
        Magic
    }

    public State TakeDamage(float damage, DamageType type, GameObject attacker);
    
    public void AddDebuff(Buff debuff);
}
