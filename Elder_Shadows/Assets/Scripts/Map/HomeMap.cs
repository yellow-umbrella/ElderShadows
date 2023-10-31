using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class HomeMap : MonoBehaviour
{
    private Tilemap _homeTilemap;
    private TilesHolder _tilesHolder;
    private Random rnd = new Random();
    int width = 150;
    int height = 100;
    int treeNum = 200;

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
        
        var h_x = rnd.Next(1, width);
        var h_y = rnd.Next(1, height);

        List<int> treeX = generateTreePositions(treeNum, width);
        List<int> treeY = generateTreePositions(treeNum, height);

        for (var h = 0; h < height; h++)
        {
            for (var w = 0; w < width; w++)
            {
                if(h_x == w && h_y == h) 
                {
                    _homeTilemap.SetTile(currentCellPosition, _tilesHolder.GetHomeTile());
                }
                else if (rnd.Next(0, 60) == 10 && w != 0 && w != width-1 && h != height-1 && h != 0) 
                {
                    _homeTilemap.SetTile(currentCellPosition, _tilesHolder.GetTreeTile());
                }
                else 
                {
                    _homeTilemap.SetTile(currentCellPosition, _tilesHolder.GetBaseTile());
                }

                /*for (int x = 0; x < treeNum; x++)
                {
                    if ((int)treeX[x] == w && (int)treeY[x] == h)
                    {
                        _homeTilemap.SetTile(currentCellPosition, _tilesHolder.GetTreeTile());
                    }
                }*/

                currentCellPosition = new Vector3Int(
                    (int)(cellSize.x + currentCellPosition.x),
                    currentCellPosition.y, origin.z);
            }
            currentCellPosition = new Vector3Int(origin.x, (int)(cellSize.y + currentCellPosition.y), origin.z);
        }

        _homeTilemap.CompressBounds();
    }

    private List<int> generateTreePositions(int num, int len) 
    {
        List<int> tx = new List<int>();

        for(int i=0; i<num; i++) 
        {
            tx.Add(rnd.Next(3, len-3));
        }

        return tx;
    }

}
