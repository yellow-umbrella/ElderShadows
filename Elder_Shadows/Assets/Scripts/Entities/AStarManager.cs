using Pathfinding;
using Pathfinding.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        yield return new WaitForEndOfFrame();
        startedScanning = true;
        AstarPath.active.Scan(AstarPath.active.data.gridGraph);
    }

    private void Update()
    {
        if (startedScanning && !AstarPath.active.isScanning)
        {
            startedScanning = false;
            SerializeSettings settings = new SerializeSettings();
            settings.nodes = true;
            byte[] bytes = AstarPath.active.data.SerializeGraphs(settings);
            File.WriteAllBytes(Application.persistentDataPath + "/gg.bytes", bytes);

            OnFinishedScanning?.Invoke();
        }
    }

}
