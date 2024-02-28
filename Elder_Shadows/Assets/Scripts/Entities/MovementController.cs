using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class MovementController : MonoBehaviour
{
    public event Action OnMoving;
    public float MoveSpeed { get; set; } = 1.0f;
    public bool ReachedEndOfPath { get; private set; }
    public bool IsGeneratingPath { get; private set; }
    public int MovementDirection 
    { 
        get
        {
            if (movementDirection == Vector2.down) return 0;
            if (movementDirection == Vector2.up) return 1;
            if (movementDirection == Vector2.right) return 2;
            if (movementDirection == Vector2.left) return 3;
            return -1;
        } 
    }

    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private float nextWaypointDist = .5f;
    private Vector2 movementDirection;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        ReachedEndOfPath = true;
    }

    /// <summary>
    /// Ask seeker to plan new path from current position to target position.
    /// </summary>
    public void SetPath(Vector2 target)
    {
        if (seeker.IsDone())
        {
            IsGeneratingPath = true;
            seeker.StartPath(transform.position, target, OnPathGenComplete);
        }
    }

    // recieve generated path and refresh current waypoint
    private void OnPathGenComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            IsGeneratingPath = false;
            ReachedEndOfPath = false;
        } else
        {
            path = null;
        }
    }

    public void MoveOnPath()
    {
        // no path
        if (path == null)
        {
            return;
        }

        // reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            ReachedEndOfPath = true;
            return;
        }
        else
        {
            ReachedEndOfPath = false;
        }

        // move on path in direction of current waypoint
        Vector2 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
        movementDirection = SnapVector(direction);
        OnMoving?.Invoke();

        // if close to current waypoint move on to next
        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDist)
        {
            currentWaypoint++;
        }
    }

    public static Vector2 SnapVector(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            dir.y = 0;
        }
        else
        {
            dir.x = 0;
        }
        return dir.normalized;
    }
}
