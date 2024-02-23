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
        attackTarget.GetComponent<IAttackable>()
            .TakeDamage(damage, currentAttack.dmgType, gameObject);
    }

    public bool CanAttack(GameObject attackTarget)
    {
        if (attackTarget.HasComponent<IAttackable>() && ChooseAttack(ref currentAttack))
        {
            return Vector2.Distance(attackTarget.transform.position, transform.position) <= currentAttack.range;
        }
        return false;
    }

    private bool ChooseAttack(ref EntityAttackSO attack)
    {
        if (info.attacks.Count == 0) { return false; }
        attack = info.attacks[Random.Range(0, info.attacks.Count)];
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
