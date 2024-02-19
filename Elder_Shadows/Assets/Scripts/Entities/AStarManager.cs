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
        StartCoroutine(ScanMap());
    }

    IEnumerator ScanMap()
    {
        startedScanning = true;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
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
