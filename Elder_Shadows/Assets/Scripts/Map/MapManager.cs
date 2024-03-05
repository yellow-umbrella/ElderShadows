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
    public const string HOME_MAP_PATH = "/map";
    public const string HOME_MAP_FLOOR_PATH = "/home_floor.json";
    public const string HOME_MAP_WALLS_PATH = "/home_walls.json";
    public const string HOME_MAP_WALLS_L2_PATH = "/home_walls2.json";
    public const string HOME_MAP_WALLS_L3_PATH = "/home_walls3.json";
    public const string HOME_MAP_OBJECTS_PATH = "/home_objects.json";

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

    private void Start()
    {
        
    }

    
    public Tilemap tilemap;
    public Tilemap collisions;
    public Tilemap collisions2;
    public Tilemap collisions3;

    private void Update(){}


    public void SaveTilemap(Tilemap tilemap, string save_name) 
    {
        BoundsInt bounds = tilemap.cellBounds;
        LevelData mapData = new LevelData();
        string FILE_NAME = "/" + save_name + ".json";

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

        string json = JsonUtility.ToJson(mapData, true);

        try
        {
            string map_path = Application.persistentDataPath + HOME_MAP_PATH;

            if (!Directory.Exists(map_path))
            {
                Directory.CreateDirectory(map_path);
            }

            File.WriteAllText(Application.persistentDataPath + HOME_MAP_PATH + FILE_NAME, json);
        }
        catch
        {
            Debug.Log("Map directory not found");
        }
    }

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
                if (go.layer == 17)
                {
                    try 
                    {
                        CustomGameObject tempObject = objects.Find(o => o.gobject.name + "(Clone)" == go.name);
                        levelObject.objects.Add(tempObject.id);
                        levelObject.poses.Add(Vector3Int.FloorToInt(go.transform.position));
                    }
                    catch 
                    {
                        //Debug.Log("Map objects not saved");
                    }

                }

            }
        }
        try
        {
            string map_path = Application.persistentDataPath + HOME_MAP_PATH;
            string json = JsonUtility.ToJson(levelObject, true);

            if (!Directory.Exists(map_path))
            {
                Directory.CreateDirectory(map_path);
            }

            File.WriteAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_OBJECTS_PATH, json);
            Debug.Log("Objects save ended");
        }
        catch
        {
            Debug.Log("Map directory not found");
        }
        

        
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
        //string json = File.ReadAllText(Application.dataPath + "/homeObjects.json");
        string json = File.ReadAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_OBJECTS_PATH);
        LevelObject o_data = JsonUtility.FromJson<LevelObject>(json);

        ClearLevelObjects("Tree");

        for (int i = 0; i < o_data.objects.Count; i++)
        {
            GameObject plant = (GameObject)Instantiate(objects.Find(o => o.id == o_data.objects[i]).gobject, o_data.poses[i], Quaternion.identity);
            plant.transform.parent = transform;

            plant.layer = 17;
        }
    }

    public void LoadTilemap(Tilemap tilemap, string save_name) 
    {
        string FILE_NAME = "/" + save_name + ".json";
        string json = File.ReadAllText(Application.persistentDataPath + HOME_MAP_PATH + FILE_NAME);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        tilemap.ClearAllTiles();

        for (int i = 0; i < data.tiles.Count; i++)
        {
            tilemap.SetTile(data.poses[i], tiles.Find(t => t.id == data.tiles[i]).tile);
        }
    }

    public void Savelevel()
    {
        SaveTilemap(tilemap, "home_floor");
        SaveTilemap(collisions, "home_walls_l1");
        SaveTilemap(collisions2, "home_walls_l2");
        SaveTilemap(collisions3, "home_walls_l3");


        SaveLevelObjects();
        //Debug.Log("Level was saved");
    }

    public void LoadLevel()
    {
        LoadTilemap(tilemap, "home_floor");
        LoadTilemap(collisions, "home_walls_l1");
        LoadTilemap(collisions2, "home_walls_l2");
        LoadTilemap(collisions3, "home_walls_l3");

        LoadObjects();
    }
}


