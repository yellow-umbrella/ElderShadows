using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public CharacterData characterData;
    private bool canAttack = true;
    
    public void TryAttack()
    {
        IAttackable target = Physics2D.OverlapCircle(transform.position, characterData.AttackRange).gameObject.GetComponent<IAttackable>();
        if (canAttack)
        {
            target.TakeDamage(characterData.PhysDmg, this.gameObject);
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(characterData.AtkSpd);
        canAttack = true;
    }
}
