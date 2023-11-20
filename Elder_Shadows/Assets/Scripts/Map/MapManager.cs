using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;

public class LevelData
{
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> poses = new List<Vector3Int>();
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    
    public Tilemap tilemap;
    public Tilemap collisions;


    private void Update()
    {
        //save level when pressing Ctrl + A
        //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A)) Savelevel();
        //load level when pressing Ctrl + L
        //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) LoadLevel();
    }

    public void Savelevel()
    {
        //get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;
        BoundsInt c_bounds = collisions.cellBounds;

        //create a new leveldata
        LevelData mapData = new LevelData();
        LevelData collisionsData = new LevelData();

        //loop trougth the bounds of the tilemap
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                

                
                if (temp != null)
                {
                    //add the values to the leveldata
                    mapData.tiles.Add(temp);
                    mapData.poses.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        for (int x = c_bounds.min.x; x < c_bounds.max.x; x++)
        {
            for (int y = c_bounds.min.y; y < c_bounds.max.y; y++)
            {
                
                TileBase c_temp = collisions.GetTile(new Vector3Int(x, y, 0));
                
                if (c_temp != null)
                {
                    
                    collisionsData.tiles.Add(c_temp);
                    collisionsData.poses.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        
        string json = JsonUtility.ToJson(mapData, true);
        string c_json = JsonUtility.ToJson(collisionsData, true);
        File.WriteAllText(Application.dataPath + "/homeLevel.json", json);
        File.WriteAllText(Application.dataPath + "/homeCollisionsLevel.json", c_json);

        
        //Debug.Log("Level was saved");
    }

    public void LoadLevel()
    {
        //load the json file to a leveldata
        string json = File.ReadAllText(Application.dataPath + "/homeLevel.json");
        string c_json = File.ReadAllText(Application.dataPath + "/homeCollisionsLevel.json");
        string o_json = File.ReadAllText(Application.dataPath + "/homeObjects.json");

        LevelData data = JsonUtility.FromJson<LevelData>(json);
        LevelData c_data = JsonUtility.FromJson<LevelData>(c_json);
        MapObjects o_data = JsonUtility.FromJson<MapObjects>(o_json);

        //clear the tilemap
        tilemap.ClearAllTiles();
        collisions.ClearAllTiles();

        //place the tiles
        for (int i = 0; i < data.tiles.Count; i++)
        {
            tilemap.SetTile(data.poses[i], data.tiles[i]);
        }
        for (int i = 0; i < c_data.tiles.Count; i++)
        {
            collisions.SetTile(c_data.poses[i], c_data.tiles[i]);
        }
        for (int i = 0; i < o_data.gameObject.Count; i++) 
        {
            GameObject plant = (GameObject)Instantiate(o_data.gameObject[i], o_data.position[i], Quaternion.identity);
            plant.transform.parent = transform;

            plant.layer = 3;
        }

            
            //Debug.Log("Level was loaded");
    }
}


