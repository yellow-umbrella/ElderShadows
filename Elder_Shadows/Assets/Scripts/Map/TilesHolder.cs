using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesHolder : MonoBehaviour
{
    private RuleTile _baseTile;
    private Tile homeTile;
    private Tile treeTile;

    private void Awake()
    {
        _baseTile = (RuleTile)Resources.Load("GrassRuleTile", typeof(RuleTile));
        homeTile = (Tile)Resources.Load("tileset-sliced_107", typeof(Tile));
        treeTile = (Tile)Resources.Load("tree-orange", typeof(Tile));
    }
    public RuleTile GetBaseTile()
    {
        return _baseTile;
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
