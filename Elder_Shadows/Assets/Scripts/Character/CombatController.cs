using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public CharacterData characterData;
    public CharacterDataManager dataManager;
    private bool canAttack = true;

    public void TryAttack()
    {
        LayerMask layer1 = LayerMask.GetMask("Predator");
        LayerMask layer2 = LayerMask.GetMask("Herbivore");
        LayerMask layer3 = LayerMask.GetMask("EvilCreature");
        LayerMask layer4 = LayerMask.GetMask("GoodCreature");
        LayerMask finalMask = layer1 | layer2 | layer3 | layer4;
        BaseEntity target = Physics2D.OverlapCircle(transform.position, characterData.AttackRange, finalMask)?.gameObject.GetComponent<BaseEntity>();
        int expForKill = 0;
        if (target != null)
        {
            expForKill = target.ExpForKill;
            if (canAttack)
            {
                if (target?.TakeDamage(characterData.PhysDmg, this.gameObject) == IAttackable.State.Dead)
                {
                    dataManager.AddExperience(expForKill);
                }
                
                StartCoroutine(AttackCooldown());
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(characterData.AtkSpd);
        canAttack = true;
    }
}
