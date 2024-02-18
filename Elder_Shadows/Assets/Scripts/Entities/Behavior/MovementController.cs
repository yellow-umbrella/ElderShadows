using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Seeker seeker;
    private Path path;

    private int currentWaypoint = 0;
    private float nextWaypointDist = .5f;

    public bool ReachedEndOfPath { get; private set; }
    public bool IsGeneratingPath { get; private set; }

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        ReachedEndOfPath = true;
    }

    // ask seeker to plan new path
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
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // if close to current waypoint move on to next
        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDist)
        {
            currentWaypoint++;
        }
    }

}
