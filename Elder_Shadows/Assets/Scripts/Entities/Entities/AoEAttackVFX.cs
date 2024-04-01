using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAttackVFX : MonoBehaviour, EntityAttackVFX
{
    private AoEAttackSO attack;

    public void OnEndAttack()
    {
        Destroy(gameObject);
    }

    public void OnInflictDamage()
    {
        gameObject.SetActive(false);
    }

    public void SetupVFX(EntityAttackSO attack)
    {
        this.attack = attack as AoEAttackSO;
        transform.localScale = new Vector3(this.attack.range * 2, this.attack.range * 2, 1);
    }
}
