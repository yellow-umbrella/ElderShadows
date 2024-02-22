using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInSeeingRangeCheck : Node
{
    private BaseEntity entity;
    private float checkCooldown = .5f;
    private bool canCheck = true;
    private List<GameObject> enemies;

    /// <summary>
    /// Key to List of GameObjects of all enemies nearby
    /// </summary>
    public const string ENEMIES = "enemies";

    public EnemyInSeeingRangeCheck(BaseEntity entity)
    {
        this.entity = entity;
        enemies = new List<GameObject>();
    }

    public override NodeState Evaluate()
    {
        // find all enemies nearby
        if (canCheck)
        {
            enemies = entity.FindEnemies();
            entity.StartCoroutine(CheckCooldown());
        }

        // add attacker to enemies
        GameObject hitBy = entity.HitByEnemy();
        if (hitBy != null && !enemies.Contains(hitBy))
        {
            enemies.Add(hitBy);
        }

        GetRoot().SetData(ENEMIES, enemies);
        if (enemies.Count > 0)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

    private IEnumerator CheckCooldown()
    {
        canCheck = false;
        yield return new WaitForSeconds(checkCooldown);
        canCheck = true;
    }
}