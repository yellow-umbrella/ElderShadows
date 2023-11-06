using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEntity : MonoBehaviour, IAttackable
{
    public event EventHandler OnDeath;

    public enum Behavior
    {
        Aggressive = 0,
        Defensive = 1,
        Relaxed = 2,
        Scared = 3,
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

    [System.Serializable]
    private struct Item
    {
        public GameObject prefab;
        public float chance;
    }

    [SerializeField] private int health = 10;
    [SerializeField] private int expForKill = 5;
    [SerializeField] private List<Item> dropItems = new List<Item>();

    public int ExpForKill
    {
        get { return expForKill; }
    }

    [SerializeField] private float walkingSpeed = 1;
    [SerializeField] private float runningSpeed = 2;

    [Header("Attack parameters")]
    [SerializeField] private int damage = 2;
    [SerializeField] private float attackCooldown = 3;
    [SerializeField] private Collider2D attackRange;
    [SerializeField] private Collider2D seeingRange;

    [SerializeField] private List<LayerMaskToBehaviorType> reactionMask;
    private List<LayerMaskToIBehavior> attackBehavior;
    [SerializeField] private Behavior reactionToPlayer;
    [SerializeField] private Behavior reactionToTrustedPlayer;
    [SerializeField] private int trustRequired;

    [Header("Wandering parameters")]
    [SerializeField] private float wanderingRadius = 4;
    [SerializeField] private float timeBetweenTurns = .2f;
    [SerializeField] private float turnSpeed = 5;
    [SerializeField] private float turnAngleRange = 135;

    private Vector3 spawnPosition;

    private Vector3 wanderingCenter;
    private Vector3 currentMovementDirection;
    private Vector3 nextMovementDirection;

    private float timeToNextTurn;

    private List<Collider2D> intruders = new List<Collider2D>();

    private bool canAttack = true;
    private bool isWandering = false;

    private float timeBetweenAggroChecks = 1;
    private float timeToNextAggroCheck;

    private Dictionary<Behavior, IAttackBehavior> matchedBehavior;

    private const int characterLayer = 7;

    private void Start()
    {
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

        spawnPosition = transform.position;
    }

    private void Update()
    {
        if (!AttackBehavior())
        {
            WanderAround();
        } else
        {
            isWandering = false;
        }
    }

    private void FixedUpdate()
    {
        CheckForIntruders();
    }

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

    protected void CheckForIntruders()
    {
        // tick timer
        timeToNextAggroCheck -= Time.deltaTime;
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
                // TODO? check that it is not the same creature
                behavior.behavior.OnSee(potentialTarget);

            }
        }
    }

    public bool CanSee(GameObject target)
    {
        return seeingRange.OverlapPoint(target.transform.position);
    }

    public IAttackable.State TakeDamage(int damage, GameObject attacker)
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
                Instantiate(item.prefab, pos, Quaternion.identity);
            }
        }
    }

    public void RunTowards(Vector3 target)
    {
        MoveTowards(target, runningSpeed);
    }

    public void WalkTowards(Vector3 target)
    {
        MoveTowards(target, walkingSpeed);
    }

    private bool MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        currentMovementDirection = direction.normalized;
        Vector3 movementVector =
            currentMovementDirection * Mathf.Min(speed * Time.deltaTime, direction.magnitude);

        transform.position += movementVector;

        return Vector3.Distance(transform.position, target) <= Vector3.kEpsilon;
    }

    // Simulate random movement inside circle
    public void WanderAround()
    {
        if (!isWandering)
        {
            InitWandering(transform.position);
        }

        timeToNextTurn -= Time.deltaTime;
        if (timeToNextTurn <= 0)
        {
            timeToNextTurn = timeBetweenTurns;
            Quaternion randomRotation = Quaternion.AngleAxis(
                Random.Range(-turnAngleRange, turnAngleRange), Vector3.forward);
            nextMovementDirection = randomRotation * currentMovementDirection;
        }

        currentMovementDirection = Vector2.Lerp(currentMovementDirection, nextMovementDirection,
                                    Time.deltaTime * turnSpeed).normalized;

        Vector3 movementVector = currentMovementDirection * walkingSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movementVector;

        if (Vector3.Distance(newPosition, wanderingCenter) > wanderingRadius)
        // new position is outside the circle
        {
            // rotation of new coordinate system
            float angle = Mathf.Atan2(movementVector.y, movementVector.x);
            Quaternion rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle - 90f, Vector3.back);

            // calculate circle center coordinates relatively to current position
            Vector3 newCenter = wanderingCenter - transform.position;
            newCenter = rotation * newCenter;

            // find coordinates of intersection of circle and movement vector
            float y = newCenter.y + Mathf.Sqrt(wanderingRadius * wanderingRadius - newCenter.x * newCenter.x);
            newPosition = new Vector3(0, y, 0);

            // convert newPosition coordinates back to global
            newPosition = Quaternion.Inverse(rotation) * newPosition;
            newPosition += transform.position;

            // reflect movement direction
            currentMovementDirection = Vector3.Reflect(currentMovementDirection,
                                        (wanderingCenter - newPosition).normalized).normalized;
            nextMovementDirection = currentMovementDirection;
        }

        transform.position = newPosition;
    }

    private void InitWandering(Vector3 center)
    {
        timeToNextTurn = timeBetweenTurns;
        wanderingCenter = center;
        currentMovementDirection = Random.insideUnitCircle.normalized;
        nextMovementDirection = currentMovementDirection;
        isWandering = true;
    }
}
