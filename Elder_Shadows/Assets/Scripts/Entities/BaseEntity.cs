using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEntity : MonoBehaviour, IAttackable
{
    public event Action<BaseEntity> OnDeath;
    public event Action<BaseEntity> OnTooFar;
    public event Action<EntityAttackSO.AttackType> OnStartAttack;

    public enum ModifierType
    {
        Health,
        Damage,
        Speed
    }

    [Serializable]
    public struct Modifier
    {
        public ModifierType type;
        public float chance;
        public float modifier;

        public void Apply(BaseEntity entity)
        {
            switch (type)
            {
                case ModifierType.Health:
                    entity.Health = entity.Health * modifier;
                    break;
                case ModifierType.Damage:
                    entity.physDmg = entity.physDmg * modifier;
                    entity.magDmg = entity.magDmg * modifier;
                    break;
                case ModifierType.Speed:
                    if (entity.gameObject.TryGetComponent(out MovementController controller))
                    {
                        controller.MoveSpeed = controller.MoveSpeed * modifier;
                    }
                    break;
            }
        }
    }

    public enum Behavior
    {
        Aggressive = 0,
        Defensive = 1,
        Relaxed = 2,
        Scared = 3,
    }

    [Serializable]
    public struct Drop
    {
        public ItemObject itemInfo;
        public float chance;
    }


    public EntityInfoSO Info { get { return info; } }
    public string ID { get { return info.id; } }
    public float Health { get; private set; }
    public int ExpForKill { get { return info.expForKill; } }
    public Behavior CurrentReaction { get { return info.reactionToPlayer;} }
    public bool IsModified { get; private set; } = false;
    public float MaxDistanceFromPlayer { get; set; } = float.MaxValue;
    public Vector2 LookDirection { get; private set; }
    public bool IsAttacking { get; set; }
    public bool CanInflictDamage { get; set; }

    [SerializeField] private EntityInfoSO info;
    [SerializeField] private Collider2D seeingRange;

    private float physDmg;
    private float magDmg;

    private const int characterLayer = 7;
    private GameObject hitBy = null;
    private EntityAttackSO currentAttack = null;

    private void Awake()
    {
        Health = info.health;
        physDmg = info.physDmg;
        magDmg = info.magDmg;

        GetComponent<MovementController>().MoveSpeed = info.speed;

        foreach (Modifier modifier in info.possibleModifiers)
        {
            float r = Random.value;
            if (r < modifier.chance)
            {
                modifier.Apply(this);
                IsModified = true;
            }
        }
    }

    private void Update()
    {
        if (Vector2.Distance(CharacterController.instance.transform.position, transform.position) > MaxDistanceFromPlayer)
        {
            OnTooFar?.Invoke(this);
        }
    }
    
    public void Attack(GameObject attackTarget)
    {
        if (attackTarget == null) { return; }
        float damage = 0;
        switch (currentAttack.dmgType)
        {
            case IAttackable.DamageType.Physical:
                damage = physDmg;
                break;
            case IAttackable.DamageType.Magic: 
                damage = magDmg; 
                break;
        }
        damage *= currentAttack.dmgMultiplier;
        IAttackable target = attackTarget.GetComponent<IAttackable>();
        switch (currentAttack.attackType)
        {
            case EntityAttackSO.AttackType.Targeted:
                TargetedAttack(attackTarget.transform, target, (TargetedAttackSO)currentAttack, damage);
                break;
            case EntityAttackSO.AttackType.Ranged:
                RangedAttack(attackTarget.transform, (RangedAttackSO)currentAttack, damage);
                break;
            case EntityAttackSO.AttackType.AoE:
                AoEAttack(attackTarget, (AoEAttackSO)currentAttack, damage);
                break;
            case EntityAttackSO.AttackType.Summon:
                SummonAttack((SummonAttackSO)currentAttack);
                break;
        }
    }

    private void TargetedAttack(Transform targetObj, IAttackable target, TargetedAttackSO attack, float damage)
    {
        if (Vector2.Distance(targetObj.position, transform.position) <= attack.range)
        {
            Debug.Log($"{gameObject} performed targeted attack on {targetObj.gameObject}");
            target.TakeDamage(damage, attack.dmgType, gameObject);
        }
    }

    private void RangedAttack(Transform targetObj, RangedAttackSO attack, float damage)
    {
        Debug.Log($"{gameObject} performed ranged attack on {targetObj.gameObject}");
        GameObject projectile = Instantiate(attack.projectile, transform.position, Quaternion.identity);
        // setup projectile
    }

    private void SummonAttack(SummonAttackSO attack)
    {
        for (int i = 0; i < attack.amount; i++)
        {
            EntitySpawner.Instance.SpawnEntity(attack.entities[Random.Range(0,attack.entities.Count)], transform.position);
        }
        Debug.Log($"{gameObject} performed summon attack");
    }

    private void AoEAttack(GameObject targetObj, AoEAttackSO attack, float damage)
    {
        LayerMask mask = info.aggressionTargets | (1 << targetObj.layer);
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(mask);
        contactFilter.useTriggers = false;

        List<Collider2D> targets = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, attack.range, contactFilter, targets);
        Debug.Log($"{gameObject} performed AoE attack on {attack.range} range");
        foreach (Collider2D target in targets)
        {
            if (target.TryGetComponent(out IAttackable attackableTarget))
            {
                attackableTarget.TakeDamage(damage, attack.dmgType, gameObject);
                Debug.Log($"AoE attack hit {attackableTarget}");
            }
        }

    }

    public void StartAttack(GameObject attackTarget)
    {
        IsAttacking = true;
        CanInflictDamage = false; // waiting for right moment in animation
        Vector2 dir = attackTarget.transform.position - transform.position;
        LookDirection = MovementController.SnapVector(dir);
        OnStartAttack?.Invoke(currentAttack.attackType);
    }

    public bool CanAttack(GameObject attackTarget)
    {
        if (attackTarget.HasComponent<IAttackable>())
        {
            switch (currentAttack.attackType)
            {
                case EntityAttackSO.AttackType.Targeted:
                    return Vector2.Distance(attackTarget.transform.position, transform.position) 
                        <= ((TargetedAttackSO)currentAttack).range;
                case EntityAttackSO.AttackType.Ranged:
                    return Vector2.Distance(attackTarget.transform.position, transform.position)
                        <= ((RangedAttackSO)currentAttack).range;
                case EntityAttackSO.AttackType.AoE:
                    return Vector2.Distance(attackTarget.transform.position, transform.position)
                        <= ((AoEAttackSO)currentAttack).range;
                case EntityAttackSO.AttackType.Summon:
                    return true;
            }
        }
        return false;
    }

    public bool ChooseAttack(List<EntityAttackSO> attacks)
    {
        if (info.attacks.Count == 0) { return false; }
        currentAttack = attacks[Random.Range(0, attacks.Count)];
        return true;
    }

    public IAttackable.State TakeDamage(float damage, IAttackable.DamageType type, GameObject attacker)
    {
        float resistance = 0;
        switch (type)
        {
            case IAttackable.DamageType.Physical:
                resistance = info.physRes;
                break;
            case IAttackable.DamageType.Magic:
                resistance = info.magRes;
                break;
        }

        Health -= Mathf.Max(0, damage - resistance);

        if (Health <= 0)
        {
            Die();
            return IAttackable.State.Dead;
        }

        hitBy = attacker;
        return IAttackable.State.Alive;
    }

    public void AddDebuff(Buff debuff)
    {
        
    }

    [ContextMenu("Die")]
    protected void Die()
    {
        OnDeath?.Invoke(this);
        DropItems();
        Destroy(gameObject);
    }

    private void DropItems()
    {
        foreach(var item in info.dropItems)
        {
            if (Random.value <= item.chance)
            {
                float offset = 1f;
                Vector2 pos = new Vector2(transform.position.x + Random.Range(-offset, offset), 
                    transform.position.y + Random.Range(-offset, offset));
                GroundItem groundItem = new GameObject().AddComponent<GroundItem>();
                groundItem.gameObject.AddComponent<SpriteRenderer>();
                groundItem.item = item.itemInfo;
                Instantiate(groundItem, pos, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Finds all aggression targets, that entity can see.
    /// </summary>
    public List<GameObject> FindTargets()
    {
        LayerMask mask = info.aggressionTargets;
        if (CurrentReaction == Behavior.Aggressive)
        {
            mask = mask | (1 << characterLayer);
        }

        return FindGameObjects(mask);
    }

    /// <summary>
    /// Finds all scary enemies, that entity can see.
    /// </summary>
    public List<GameObject> FindEnemies()
    {
        LayerMask mask = info.scaryEnemies;
        if (CurrentReaction == Behavior.Scared)
        {
            mask = mask | (1 << characterLayer);
        }

        return FindGameObjects(mask);
    }

    private List<GameObject> FindGameObjects(LayerMask mask)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(mask);
        contactFilter.useTriggers = false;

        List<Collider2D> colliders = new List<Collider2D>();
        seeingRange.OverlapCollider(contactFilter, colliders);

        List<GameObject> objects = new List<GameObject>();
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                objects.Add(collider.gameObject);
            }
        }

        return objects;
    }

    /// <summary>
    /// Checks if entity was hit by one of the targets.
    /// </summary>
    public GameObject HitByTarget()
    {
        GameObject target = null;
        if (hitBy != null)
        {
            LayerMask mask = info.targets | info.aggressionTargets;
            if (CurrentReaction == Behavior.Defensive || CurrentReaction == Behavior.Aggressive)
            {
                mask = mask | (1 << characterLayer);
            }
            if (mask.LayerInMask(hitBy.layer))
            {
                target = hitBy;
                hitBy = null;
            }
        }
        return target;
    }

    /// <summary>
    /// Checks if entity was hit by one of the enemies.
    /// </summary>
    public GameObject HitByEnemy()
    {
        GameObject enemy = null;
        if (hitBy != null)
        {
            LayerMask mask = info.enemies | info.scaryEnemies;
            if (CurrentReaction == Behavior.Relaxed || CurrentReaction == Behavior.Scared)
            {
                mask = mask | (1 << characterLayer);
            }
            if (mask.LayerInMask(hitBy.layer))
            {
                enemy = hitBy;
                hitBy = null;
            }
        }
        return enemy;
    }

    /// <summary>
    /// Checks if target's collider is touching seeing range collider
    /// </summary>
    public bool CanSee(GameObject target)
    {
        return seeingRange.OverlapPoint(target.transform.position);
    }
}
