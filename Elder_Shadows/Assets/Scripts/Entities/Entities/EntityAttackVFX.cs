using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EntityAttackVFX
{
    public void SetupVFX(EntityAttackSO attack);
    public void OnInflictDamage();
    public void OnEndAttack();
}
