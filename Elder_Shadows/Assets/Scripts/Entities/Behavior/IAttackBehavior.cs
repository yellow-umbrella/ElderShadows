using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackBehavior
{
    public void OnHit(GameObject other);
    public void OnSee(GameObject other);
    public bool Behave();
}
