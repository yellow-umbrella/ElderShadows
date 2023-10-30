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

    public State TakeDamage(int damage, GameObject attacker);
}
