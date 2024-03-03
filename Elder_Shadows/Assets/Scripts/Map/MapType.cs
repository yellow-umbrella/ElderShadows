using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New MapType", menuName = "LevelEditor/MapType")]
public class MapType : ScriptableObject
{
    public float humidityNum;
    public float altitudeNum;

    public float vegetationChance;
    public float rocksChance;

    public TileBase[] forestTiles;
    public TileBase[] waterTiles;
    public TileBase[] moutainTile;

    public GameObject[] forestVegetation;
    public GameObject[] forestRocks;
}
