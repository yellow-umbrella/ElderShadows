using MyBox;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Seeker))]
public class BaseEntity : MonoBehaviour, IAttackable
{
    public event EventHandler OnDeath;
    public event Action OnReachedEndOfPath;

    public enum ModifierType
    {
        Health,
        Damage,
        Speed
    }

    [System.Serializable]
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

    [System.Serializable]
    private struct Item
    {
        public ItemObject itemInfo;
        public float chance;
    }
    
    [System.Serializable]
    private struct LayerMaskToBehaviorType
    {
        public LayerMask intrudersMask;
        public Behavior behavior;
    }

    private struct LayerMaskToIBehavior
    {
        public LayerMask intrudersMask;
        public IAttackBehavior behavior;
    }

    [SerializeField] private EntityInfoSO info;
    [SerializeField] private float health = 10;
    [SerializeField] private float moveSpeed;

    [SerializeField] private List<Modifier> possibleModifiers;

    public float Health
    {
        get { return health; }
    }

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
    [SerializeField] private List<LayerMaskToBehaviorType> reactionMask;
    [SerializeField] private Behavior reactionToPlayer;
    [SerializeField] private Behavior reactionToTrustedPlayer;
    [SerializeField] private int trustRequired;

    [Foldout("Wandering parameters", true)]
    [SerializeField] private float wanderingRadius = 4;

    // attack parameters
    private bool canAttack = true;
    private float timeBetweenAggroChecks = 1;
    private float timeToNextAggroCheck;
    private List<Collider2D> intruders = new List<Collider2D>();

    // behavior parameters
    private Dictionary<Behavior, IAttackBehavior> matchedBehavior;
    private List<LayerMaskToIBehavior> attackBehavior;
    private IIdleBehavior idleBehavior;
    private const int characterLayer = 7;

    // pathfinding parameters
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private float nextWaypointDist = .5f;
    private bool reachedEndOfPath = true;
    public bool ReachedEndOfPath 
    { 
        get {  return reachedEndOfPath; } 
        private set 
        {
            if (reachedEndOfPath != value && value)
            {
                OnReachedEndOfPath?.Invoke();
            }
            reachedEndOfPath = value;
        }
    }
    private float timeBetweenPathGen = 1f;
    private float nextPathGen;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();

        foreach (Modifier modifier in possibleModifiers)
        {
            float r = Random.value;
            if (r < modifier.chance)
            {
                modifier.Apply(this);
            }
        }

        matchedBehavior =
            new Dictionary<Behavior, IAttackBehavior>()
            {
                {Behavior.Aggressive, new AggressiveBehavior(this) },
                {Behavior.Defensive, new DefensiveBehavior(this) },
                {Behavior.Relaxed, new RelaxedBehavior(this) },
                {Behavior.Scared, new ScaredBehavior(this) },
            };

        attackBehavior = new List<LayerMaskToIBehavior>
        {
            new LayerMaskToIBehavior()
            {
                intrudersMask = (1 << characterLayer),
                behavior = matchedBehavior[reactionToPlayer]
            }
        };

        foreach (var reaction in reactionMask)
        {
            attackBehavior.Add(new LayerMaskToIBehavior()
            {
                intrudersMask = reaction.intrudersMask,
                behavior = matchedBehavior[reaction.behavior]
            });
        }

        idleBehavior = new WanderingBehavior(this, wanderingRadius);
    }

    private void Update()
    {
        if (!AttackBehavior())
        {
            idleBehavior.Behave();
        }
        MoveOnPath();
        /*if (this.gameObject.tag != "Untagged")
        {
            Die();
        }*/
    }

    private void FixedUpdate()
    {
        CheckForIntruders();
    }

    // handling attack

    private bool AttackBehavior()
    {
        foreach (var behavior in attackBehavior)
        {
            if (behavior.behavior.Behave())
            {
                return true;
            }
        }
        return false;
    }

    public bool TryAttack(GameObject attackTarget)
    {
        if (attackRange.OverlapPoint(attackTarget.transform.position) && canAttack)
        {
            attackTarget.GetComponent<IAttackable>().TakeDamage(damage, this.gameObject);
            StartCoroutine(AttackCooldown());
            return true;
        }
        return false;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // finding enemies around

    protected void CheckForIntruders()
    {
        // tick timer
        timeToNextAggroCheck -= Time.fixedDeltaTime;
        if (timeToNextAggroCheck <= 0)
        {
            timeToNextAggroCheck = timeBetweenAggroChecks;
        } else
        {
            return;
        }

        // check every group of intruders and react to them
        foreach (var behavior in attackBehavior)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(behavior.intrudersMask);
            contactFilter.useTriggers = true;

            seeingRange.OverlapCollider(contactFilter, intruders);

            foreach (Collider2D collider in intruders)
            {
                GameObject potentialTarget = collider.gameObject;
                behavior.behavior.OnSee(potentialTarget);
            }
        }
    }

    public bool CanSee(GameObject target)
    {
        return seeingRange.OverlapPoint(target.transform.position);
    }

    // handling taking damage and death

    public IAttackable.State TakeDamage(float damage, GameObject attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
            return IAttackable.State.Dead;
        } else
        {
            foreach (var behavior in attackBehavior)
            {
                if (behavior.intrudersMask.LayerInMask(attacker.layer))
                {
                    behavior.behavior.OnHit(attacker);
                    break;
                }

            } 
        }
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

    // movement using A* Pathfinding project

    public void MoveTowards(Vector3 target)
    {
        SetPath(target);
    }

    private void SetPath(Vector2 target)
    {
        if (seeker.IsDone() && (Time.time > nextPathGen || ReachedEndOfPath))
        {
            seeker.StartPath(transform.position, target, OnPathGenComplete);
            nextPathGen = Time.time + timeBetweenPathGen;
        }
    }

    private void MoveOnPath()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            ReachedEndOfPath = true;
            return;
        } else
        {
            ReachedEndOfPath = false;
        }

        Vector2 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);

        if (dist < nextWaypointDist)
        {
            currentWaypoint++;
        }
    }

    private void OnPathGenComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
