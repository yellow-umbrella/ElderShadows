using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HomeMap : MonoBehaviour
{
    private Tilemap _homeTilemap;
    private TilesHolder _tilesHolder;

    private void Awake()
    {
        _homeTilemap = GetComponent<Tilemap>();
        _tilesHolder = GetComponent<TilesHolder>();
    }

    private void Start()
    {
        var origin = _homeTilemap.origin;
        var cellSize = _homeTilemap.cellSize;
        _homeTilemap.ClearAllTiles();
        var currentCellPosition = origin;
        var width = 200;
        var height = 100;
        for (var h = 0; h < height; h++)
        {
            for (var w = 0; w < width; w++)
            {
                _homeTilemap.SetTile(currentCellPosition, _tilesHolder.GetBaseTile());
                currentCellPosition = new Vector3Int(
                    (int)(cellSize.x + currentCellPosition.x),
                    currentCellPosition.y, origin.z);
            }
            currentCellPosition = new Vector3Int(origin.x, (int)(cellSize.y + currentCellPosition.y), origin.z);
        }
        _homeTilemap.CompressBounds();
    }
}
