using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public List<GameObject> gameObject = new List<GameObject>();
    public List<Vector3> position = new List<Vector3>();

    public MapObject(List<GameObject> gameObject, List<Vector3> position)
    {
        this.gameObject = gameObject;
        this.position = position;
    }
}
