using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public CharacterData characterData;
    private bool canAttack = true;

    public void TryAttack()
    {
        Debug.Log("Trying To Attack");
        LayerMask layer1 = LayerMask.GetMask("Predator");
        LayerMask layer2 = LayerMask.GetMask("Herbivore");
        LayerMask layer3 = LayerMask.GetMask("EvilCreature");
        LayerMask layer4 = LayerMask.GetMask("GoodCreature");
        LayerMask finalMask = layer1 | layer2 | layer3 | layer4;
        Collider2D target = Physics2D.OverlapCircle(transform.position, characterData.AttackRange, finalMask);
        if (canAttack)
        {
            Debug.Log("Attacking " + target.gameObject.name);
            target?.gameObject.GetComponent<IAttackable>().TakeDamage(characterData.PhysDmg, this.gameObject);
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
