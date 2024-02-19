using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class CombatController : MonoBehaviour
{
    public CharacterData characterData;
    public CharacterDataManager dataManager;
    private bool canAttack = true;
    public UnityEvent<string> onEnemyKilled;
    public SkillsDataBase skilldb;

    public void TryAttack()
    {
        LayerMask layer1 = LayerMask.GetMask("Predator");
        LayerMask layer2 = LayerMask.GetMask("Herbivore");
        LayerMask layer3 = LayerMask.GetMask("EvilCreature");
        LayerMask layer4 = LayerMask.GetMask("GoodCreature");
        LayerMask finalMask = layer1 | layer2 | layer3 | layer4;
        BaseEntity target = Physics2D.OverlapCircle(transform.position, characterData.atk_range, finalMask)?.gameObject.GetComponent<BaseEntity>();
        int expForKill = 0;
        if (target != null)
        {
            expForKill = target.ExpForKill;
            if (canAttack)
            {
                if (target?.TakeDamage(characterData.phys_dmg, IAttackable.DamageType.Physical, gameObject) == IAttackable.State.Dead)
                {
                    onEnemyKilled.Invoke(target.ID);
                    dataManager.AddExperience(expForKill);
                }
                
                StartCoroutine(AttackCooldown());
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(characterData.atk_spd);
        canAttack = true;
    }

    public void CastSkill(int skillID)
    {
        Instantiate(skilldb.GetSkill(skillID).skillPrefab);
    }
}
