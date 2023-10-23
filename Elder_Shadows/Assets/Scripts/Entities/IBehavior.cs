using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehavior
{
    public void OnHit(GameObject other);
    public void OnSee(GameObject other);
    public void Behave();
    public void InitBehavior(BaseEntity entity);
}
