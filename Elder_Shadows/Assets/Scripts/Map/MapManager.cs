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

    private void Update()
    {
        //save level when pressing Ctrl + A
        //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A)) Savelevel();
        //load level when pressing Ctrl + L
        //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) LoadLevel();
        //save level when pressing Ctrl + A
        //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A)) SaveLevelObjects();
    }

    // add this method to refactor SaveLevel
    public void SaveData() { }

    public void Savelevel()
    {
        //get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;
        BoundsInt c_bounds = collisions.cellBounds;
        BoundsInt c_bounds2 = collisions2.cellBounds;
        BoundsInt c_bounds3 = collisions3.cellBounds;

        //create a new leveldata
        LevelData mapData = new LevelData();
        LevelData collisionsData = new LevelData();
        LevelData collisionsData2 = new LevelData();
        LevelData collisionsData3 = new LevelData();

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

        for (int x = c_bounds2.min.x; x < c_bounds2.max.x; x++)
        {
            for (int y = c_bounds2.min.y; y < c_bounds2.max.y; y++)
            {

                TileBase c2_temp = collisions.GetTile(new Vector3Int(x, y, 0));
                CustomTile tempTile2 = tiles.Find(t => t.tile == c2_temp);

                if (tempTile2 != null)
                {

                    collisionsData2.tiles.Add(tempTile2.id);
                    collisionsData2.poses.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        for (int x = c_bounds3.min.x; x < c_bounds3.max.x; x++)
        {
            for (int y = c_bounds3.min.y; y < c_bounds3.max.y; y++)
            {

                TileBase c3_temp = collisions.GetTile(new Vector3Int(x, y, 0));
                CustomTile tempTile3 = tiles.Find(t => t.tile == c3_temp);

                if (tempTile3 != null)
                {

                    collisionsData3.tiles.Add(tempTile3.id);
                    collisionsData3.poses.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        //SaveLevelObjects();

        string json = JsonUtility.ToJson(mapData, true);
        string c_json = JsonUtility.ToJson(collisionsData, true);
        string c2_json = JsonUtility.ToJson(collisionsData2, true);
        string c3_json = JsonUtility.ToJson(collisionsData3, true);

        try
        {
            string map_path = Application.persistentDataPath + HOME_MAP_PATH;

            if (!Directory.Exists(map_path))
            {
                Directory.CreateDirectory(map_path);
            }

            File.WriteAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_FLOOR_PATH, json);
            File.WriteAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_WALLS_PATH, c_json);
            File.WriteAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_WALLS_L2_PATH, c2_json);
            File.WriteAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_WALLS_L3_PATH, c3_json);
        }
        catch
        {
            Debug.Log("Map directory not found");
        }


        SaveLevelObjects();
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
                        Debug.Log("Map objects not saved");
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

    public void LoadLevel()
    {
        //load the json file to a leveldata
        //string json = File.ReadAllText(Application.dataPath + "/homeLevel.json");
        //string c_json = File.ReadAllText(Application.dataPath + "/homeCollisionsLevel.json");

        string json = File.ReadAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_FLOOR_PATH);
        string c_json = File.ReadAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_WALLS_PATH);
        string c2_json = File.ReadAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_WALLS_L2_PATH);
        string c3_json = File.ReadAllText(Application.persistentDataPath + HOME_MAP_PATH + HOME_MAP_WALLS_L3_PATH);

        LevelData data = JsonUtility.FromJson<LevelData>(json);
        LevelData c_data = JsonUtility.FromJson<LevelData>(c_json);
        LevelData c2_data = JsonUtility.FromJson<LevelData>(c2_json);
        LevelData c3_data = JsonUtility.FromJson<LevelData>(c3_json);

        //clear the tilemap
        tilemap.ClearAllTiles();
        collisions.ClearAllTiles();
        collisions2.ClearAllTiles();
        collisions3.ClearAllTiles();

        //place the tiles
        for (int i = 0; i < data.tiles.Count; i++)
        {
            tilemap.SetTile(data.poses[i], tiles.Find(t => t.id == data.tiles[i]).tile);
        }
        for (int i = 0; i < c_data.tiles.Count; i++)
        {
            collisions.SetTile(c_data.poses[i], tiles.Find(t => t.id == c_data.tiles[i]).tile);
        }
        for (int i = 0; i < c2_data.tiles.Count; i++)
        {
            collisions.SetTile(c2_data.poses[i], tiles.Find(t => t.id == c2_data.tiles[i]).tile);
        }
        for (int i = 0; i < c3_data.tiles.Count; i++)
        {
            collisions.SetTile(c3_data.poses[i], tiles.Find(t => t.id == c3_data.tiles[i]).tile);
        }

        LoadObjects();
    }
}


