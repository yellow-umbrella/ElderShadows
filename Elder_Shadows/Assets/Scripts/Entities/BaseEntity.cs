using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEntity : MonoBehaviour
{
    public event EventHandler OnDeath;

    [SerializeField] private int health = 10;
    [SerializeField] private int demage = 2;

    [SerializeField] private float walkingSpeed = 1;
    [SerializeField] private float runningSpeed = 2;

    // wandering parameters
    [SerializeField] private float wanderingRadius = 4;
    [SerializeField] private float timeBetweenTurns = .2f;
    [SerializeField] private float turnSpeed = 5;
    [SerializeField] private float turnAngleRange = 135;

    private Vector3 spawnPosition;

    private Vector3 currentMovementDirection;
    private Vector3 nextMovementDirection;

    private bool isWandering = false;
    private float timeToNextTurn;

    private void Start()
    {
        spawnPosition = transform.position;
        InitWandering();
    }

    private void Update()
    {
        if (isWandering)
        {
            WanderAround(spawnPosition, wanderingRadius);
        }
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
    private void Die()
    {
        Debug.Log(gameObject.ToString() + " is dead");
        OnDeath?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
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

    // random movement inside circle
    private void WanderAround(Vector3 center, float radius)
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

        if (Vector3.Distance(newPosition, center) > radius)
            // new position is outside the circle
        {
            // rotation of new coordinate system
            float angle = Mathf.Atan2(movementVector.y, movementVector.x);
            Quaternion rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle - 90f, Vector3.back);

            // calculate circle center coordinates relatively to current position
            Vector3 newCenter = center - transform.position;
            newCenter = rotation * newCenter;
            
            // find coordinates of intersection of circle and movement vector
            float y = newCenter.y + Mathf.Sqrt(radius*radius - newCenter.x*newCenter.x);
            newPosition = new Vector3(0, y, 0);
            
            // convert newPosition coordinates back to global
            newPosition = Quaternion.Inverse(rotation) * newPosition;
            newPosition += transform.position;

            // reflect movement direction
            currentMovementDirection = Vector3.Reflect(currentMovementDirection, 
                                        (center - newPosition).normalized).normalized;
            nextMovementDirection = currentMovementDirection;
        }

        transform.position = newPosition;
    }

    private void InitWandering()
    {
        isWandering = true;
        timeToNextTurn = timeBetweenTurns;

        currentMovementDirection = Random.insideUnitCircle.normalized;
        nextMovementDirection = currentMovementDirection;
    }
}
