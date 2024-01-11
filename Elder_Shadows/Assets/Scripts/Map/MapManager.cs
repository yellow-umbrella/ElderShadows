using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;

public class LevelData
{
    public List<string> tiles = new List<string>();
    public List<Vector3Int> poses = new List<Vector3Int>();
}

public class LevelObject 
{
    public List<string> objects = new List<string>();
    public List<Vector3Int> poses = new List<Vector3Int>();
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public List<CustomTile> tiles = new List<CustomTile> ();
    public List<CustomGameObject> objects = new List<CustomGameObject>();

    private MapObject mapObject;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        mapObject = GetComponent<MapObject>();
    }

    
    public Tilemap tilemap;
    public Tilemap collisions;


    private void Update()
    {
        //save level when pressing Ctrl + A
        //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A)) Savelevel();
        //load level when pressing Ctrl + L
        //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) LoadLevel();
        //save level when pressing Ctrl + A
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A)) SaveLevelObjects();
    }

    public void Savelevel()
    {
        //get the bounds of the tilemap...
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
                CustomTile tempTile = tiles.Find(t => t.tile == temp);

                
                if (tempTile != null)
                {
                    //add the values to the leveldata
                    mapData.tiles.Add(tempTile.id);
                    mapData.poses.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        for (int x = c_bounds.min.x; x < c_bounds.max.x; x++)
        {
            for (int y = c_bounds.min.y; y < c_bounds.max.y; y++)
            {
                
                TileBase c_temp = collisions.GetTile(new Vector3Int(x, y, 0));
                CustomTile tempTile = tiles.Find(t => t.tile == c_temp);

                if (tempTile != null)
                {
                    
                    collisionsData.tiles.Add(tempTile.id);
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

    // new save objects method
    public void SaveLevelObjects() 
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        List<GameObject> gameObject = new List<GameObject>();
        List<Vector3> position = new List<Vector3>();

        LevelObject levelObject = new LevelObject();

        if (allObjects != null)
        {
            foreach (GameObject go in allObjects)
            {
                if (go.tag == "Tree" || go.tag == "House")
                {
                    CustomGameObject tempObject = objects.Find(o => o.gobject.name + "(Clone)" == go.name);

                    levelObject.objects.Add(tempObject.id);
                    levelObject.poses.Add(Vector3Int.FloorToInt(go.transform.position));
                }

            }
        }

        string json = JsonUtility.ToJson(levelObject, true);
        File.WriteAllText(Application.dataPath + "/homeObjects.json", json);
        Debug.Log("LevelUpdateEnded");
    }

    public void ClearLevelObjects(string go_tag) 
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(go_tag);

        foreach (GameObject go in gos)
        {
            Destroy(go.gameObject);
        }
    }

    public void LoadObjects()
    {
        string json = File.ReadAllText(Application.dataPath + "/homeObjects.json");
        LevelObject o_data = JsonUtility.FromJson<LevelObject>(json);

        ClearLevelObjects("Tree");

        for (int i = 0; i < o_data.objects.Count; i++)
        {
            GameObject plant = (GameObject)Instantiate(objects.Find(o => o.id == o_data.objects[i]).gobject, o_data.poses[i], Quaternion.identity);
            plant.transform.parent = transform;

            plant.layer = 17;
        }
    }

    public void LoadLevel()
    {
        //load the json file to a leveldata
        string json = File.ReadAllText(Application.dataPath + "/homeLevel.json");
        string c_json = File.ReadAllText(Application.dataPath + "/homeCollisionsLevel.json");

        LevelData data = JsonUtility.FromJson<LevelData>(json);
        LevelData c_data = JsonUtility.FromJson<LevelData>(c_json);

        //clear the tilemap
        tilemap.ClearAllTiles();
        collisions.ClearAllTiles();

        //place the tiles
        for (int i = 0; i < data.tiles.Count; i++)
        {
            tilemap.SetTile(data.poses[i], tiles.Find(t => t.id == data.tiles[i]).tile);
        }
        for (int i = 0; i < c_data.tiles.Count; i++)
        {
            collisions.SetTile(c_data.poses[i], tiles.Find(t => t.id == c_data.tiles[i]).tile);
        }

        LoadObjects();
    }

    public void DeleteLevelObject(GameObject go)
    {
        string json = File.ReadAllText(Application.dataPath + "/homeObjects.json");
        LevelObject o_data = JsonUtility.FromJson<LevelObject>(json);

        foreach(Vector3Int pos in o_data.poses) 
        {
            if (pos == Vector3Int.FloorToInt(go.transform.position))
            {
                Debug.Log(pos + " deleted");
            }
        }
    }
}


