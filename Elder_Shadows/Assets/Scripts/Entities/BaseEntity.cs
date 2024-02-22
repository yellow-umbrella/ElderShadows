using MyBox;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Seeker))]
public class BaseEntity : MonoBehaviour, IAttackable
{
    public event EventHandler OnDeath;
    public event Action<BaseEntity> OnTooFar;

    public enum ModifierType
    {
        Health,
        Damage,
        Speed
    }

    [Serializable]
    private struct Modifier
    {
        public ModifierType type;
        public float chance;
        public float modifier;

        public void Apply(BaseEntity entity)
        {
            switch (type)
            {
                case ModifierType.Health:
                    entity.health = (int)(entity.health * modifier);
                    break;
                case ModifierType.Damage:
                    entity.damage = (int)(entity.damage * modifier);
                    break;
                case ModifierType.Speed:
                    entity.moveSpeed = entity.moveSpeed * modifier;
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
    private struct Item
    {
        public ItemObject itemInfo;
        public float chance;
    }

    [SerializeField] private EntityInfoSO info;
    [SerializeField] private float health = 10;
    [SerializeField] private float moveSpeed;

    [SerializeField] private List<Modifier> possibleModifiers;

    public float Health
    {
        get { return health; }
    }

    public EntityInfoSO Info { get { return info; } }
    public string ID { get { return info.id; } }

    [Foldout("Drop parameters", true)]
    [SerializeField] private int expForKill = 5;
    [SerializeField] private List<Item> dropItems = new List<Item>();
    
    public int ExpForKill
    {
        get { return expForKill; }
    }

    [Foldout("Attack parameters", true)]
    [SerializeField] private int damage = 2;
    [SerializeField] private float attackCooldown = 3;
    [SerializeField] private Collider2D attackRange;
    [SerializeField] private Collider2D seeingRange;

    [Foldout("Behavior parameters", true)]
    [SerializeField] private Behavior reactionToPlayer;
    [SerializeField] private Behavior reactionToTrustedPlayer;
    [SerializeField] private int trustRequired;
    [SerializeField] private LayerMask aggressionTargets = new LayerMask();
    [SerializeField] private LayerMask targets = new LayerMask();
    [SerializeField] private LayerMask enemies = new LayerMask();
    [SerializeField] private LayerMask scaryEnemies = new LayerMask();

    public Behavior CurrentReaction { get { return reactionToPlayer;} }
    public bool IsModified { get; private set; } = false;

    private const int characterLayer = 7;

    public float MaxDistanceFromPlayer { get; set; } = float.MaxValue;

    private GameObject hitBy = null;

    private void Awake()
    {
        foreach (Modifier modifier in possibleModifiers)
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
        attackTarget.GetComponent<IAttackable>()
            .TakeDamage(damage, IAttackable.DamageType.Physical, gameObject);
    }

    public bool CanAttack(GameObject attackTarget)
    {
        if (attackTarget.HasComponent<IAttackable>())
        {
            return attackRange.OverlapPoint(attackTarget.transform.position);
        }
        return false;
    }

    /// <summary>
    /// Finds all aggression targets, that entity can see.
    /// </summary>
    public List<GameObject> FindTargets()
    {
        LayerMask mask = aggressionTargets;
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
        LayerMask mask = scaryEnemies;
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
            LayerMask mask = targets | aggressionTargets;
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
            LayerMask mask = enemies | scaryEnemies;
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

    // handling taking damage and death

    public IAttackable.State TakeDamage(float damage, IAttackable.DamageType type, GameObject attacker)
    {
        health -= damage;
        if (health <= 0)
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
        OnDeath?.Invoke(this, EventArgs.Empty);
        DropItems();
        Destroy(gameObject);
    }

    private void DropItems()
    {
        foreach(var item in dropItems)
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
}
