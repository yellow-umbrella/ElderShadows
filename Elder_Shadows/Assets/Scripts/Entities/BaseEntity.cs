using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEntity : MonoBehaviour, IAttackable
{
    public event EventHandler OnDeath;

    [SerializeField] private int health = 10;

    [SerializeField] private float walkingSpeed = 1;
    [SerializeField] private float runningSpeed = 2;

    [Header("Attack parameters")]
    [SerializeField] private int damage = 2;
    [SerializeField] private float attackCooldown = 3;
    [SerializeField] private Collider2D attackRange;
    [SerializeField] private Collider2D aggroRange;
    [SerializeField] private LayerMask attackMask;

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
    private GameObject attackTarget;

    private bool canAttack = true;

    private float timeBetweenAggroChecks = 1;
    private float timeToNextAggroCheck;

    protected State state;

    protected enum State
    {
        Wandering,
        Attacking,
    }

    private void Start()
    {
        spawnPosition = transform.position;
        InitWandering(spawnPosition);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Wandering:
                WanderAround();
                timeToNextAggroCheck -= Time.deltaTime;
                if (timeToNextAggroCheck <= 0)
                {
                    timeToNextAggroCheck = timeBetweenAggroChecks;
                    if (CheckForIntruders(attackMask))
                    {
                        state = State.Attacking;
                    }
                }
                break;
            case State.Attacking:
                if (attackTarget != null)
                {
                    MoveTowards(attackTarget.transform.position, runningSpeed);
                    TryAttack();
                }
                else
                {
                    InitWandering(transform.position);
                }
                break;
        }
    }

    protected void TryAttack()
    {
        if (attackRange.OverlapPoint(attackTarget.transform.position) && canAttack)
        {
            attackTarget.GetComponent<IAttackable>().TakeDamage(damage);
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    protected bool CheckForIntruders(LayerMask intrudersMask)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(intrudersMask);
        contactFilter.useTriggers = true;

        aggroRange.OverlapCollider(contactFilter, intruders);

        foreach (Collider2D collider in intruders)
        {
            GameObject potentialTarget = collider.gameObject;
            if (!potentialTarget.Equals(gameObject))
            {
                if (aggroRange.OverlapPoint(potentialTarget.transform.position)
                    && potentialTarget.HasComponent<IAttackable>())
                {
                    attackTarget = potentialTarget;
                    return true;
                }
            }
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    [ContextMenu("Die")]
    protected void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    protected bool MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        currentMovementDirection = direction.normalized;
        Vector3 movementVector =
            currentMovementDirection * Mathf.Min(speed * Time.deltaTime, direction.magnitude);

        transform.position += movementVector;

        return Vector3.Distance(transform.position, target) <= Vector3.kEpsilon;
    }

    // Simulate random movement inside circle
    protected void WanderAround()
    {
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

    protected void InitWandering(Vector3 center)
    {
        state = State.Wandering;
        timeToNextTurn = timeBetweenTurns;
        wanderingCenter = center;
        currentMovementDirection = Random.insideUnitCircle.normalized;
        nextMovementDirection = currentMovementDirection;
    }
}
