using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public event Action OnFinishedScanning;

    private bool startedScanning;

    void Start()
    {
        // should be called after map and colliders are generated
        Invoke("Scan", 2);
    }

    private void Scan()
    {
        startedScanning = true;
        AstarPath.active.Scan();
    }

    private void Update()
    {
        if (startedScanning && !AstarPath.active.isScanning)
        {
            startedScanning = false;
            OnFinishedScanning?.Invoke();
        }
    }

}
