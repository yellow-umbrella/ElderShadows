using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesHolder : MonoBehaviour
{
    private RuleTile lightGrassTile;
    private Tile homeTile;
    private Tile treeTile;
    private Tile sandTile;
    private Tile waterTile;
    private Tile grassTile;

    private void Awake()
    {
        lightGrassTile = (RuleTile)Resources.Load("LightGrassRuleTile", typeof(RuleTile));
        homeTile = (Tile)Resources.Load("rock-monument", typeof(Tile));
        treeTile = (Tile)Resources.Load("tree-orange", typeof(Tile));

        sandTile = (Tile)Resources.Load("tileset_3", typeof(Tile));
        waterTile = (Tile)Resources.Load("tileset_2", typeof(Tile));
        grassTile = (Tile)Resources.Load("tileset_0", typeof(Tile));
    }


    public RuleTile GetBaseTile()
    {
        return lightGrassTile;
    }

    public Tile GetHomeTile() 
    {
        return homeTile;
    }

    public Tile GetTreeTile() 
    {
        return treeTile;
    }

    public Tile GetSandTile()
    {
        return sandTile;
    }
    public Tile GetWaterTile()
    {
        return waterTile;
    }
    public Tile GetGrassTile()
    {
        return grassTile;
    }
}
