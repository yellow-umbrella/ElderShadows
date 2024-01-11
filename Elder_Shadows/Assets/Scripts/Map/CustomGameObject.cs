using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Custom gameobject", menuName = "LevelEditor/GameObject")]
public class CustomGameObject : ScriptableObject
{
    public GameObject gobject ;
    public string id;
}
