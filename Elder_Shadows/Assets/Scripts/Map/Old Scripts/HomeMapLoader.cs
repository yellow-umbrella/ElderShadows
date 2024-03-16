using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HomeMapLoader : MonoBehaviour
{
    /*Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    [SerializeField] BoundsInt bounds;
    [SerializeField] string filename = "tilemapData.json";

    // Start is called before the first frame update
    void Start()
    {
        initTilemaps();
    }

    private void initTilemaps() {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();
        foreach (var map in maps) {
            tilemaps.Add(map.name, map);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Save() {
        List<TilemapData> data = new List<TilemapData>();
        foreach (var mapObj in tilemaps) {
            TilemapData mapData = new TilemapData();
            mapData.key = mapObj.Key;
        }
    }
    void Load() { }*/
}

[Serializable]
public class TileInfo
{
    public TileBase tile;
    public Vector3Int position;
        
        public TileInfo(TileBase tile, Vector3Int pos)
    {
        this.tile = tile;
        position = pos;
    }
}