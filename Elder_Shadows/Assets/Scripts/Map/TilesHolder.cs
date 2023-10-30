using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesHolder : MonoBehaviour
{
    private RuleTile lightGrassTile;
    private Tile homeTile;
    private Tile treeTile;

    private void Awake()
    {
        lightGrassTile = (RuleTile)Resources.Load("LightGrassRuleTile", typeof(RuleTile));
        homeTile = (Tile)Resources.Load("rock-monument", typeof(Tile));
        treeTile = (Tile)Resources.Load("tree-orange", typeof(Tile));
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
}
