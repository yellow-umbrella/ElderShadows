using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    void Start()
    {
        // should be called after map and colliders are generated
        Invoke("Scan", 2);
    }

    private void Scan()
    {
        AstarPath.active.Scan();
    }

}
