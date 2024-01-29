using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Linq;
using System.IO;

public class PerlinGeneration : MonoBehaviour
{
    public Tilemap tileMap;
    public Tilemap wallsTileMap; 
    public TileBase[] forestTiles;
    public TileBase[] waterTiles;
    public TileBase moutainTile;
    public TileBase mountainWaterTile;
    public GameObject[] forestVegetation;
    public GameObject[] forestRocks;

    public float perlin;
    public float scale;
    public float seed;

    public int xOffset;
    public int yOffset;

    public float width;
    public float height;
    
    public float vegetationChance;
    public float rocksChance;
    
    private List<GameObject> objectList = new List<GameObject>();
    private List<Vector3> posList = new List<Vector3>();
    private MapManager mapManager;
    private PlayerSpawn pSpawn;
    private HomeSpawn homeSpawn;
    private MapObject mapObject;
    //private bool firstCheck = false;

    private void Awake() 
    {
        mapManager = GetComponent<MapManager>();
        mapObject = GetComponent<MapObject>();
        pSpawn = GetComponent<PlayerSpawn>();
        homeSpawn = GetComponent<HomeSpawn>();
    }

    private void Start()
    {
        seed = Random.Range(0f, 100000f);
        GenerateWorld();

        // add code to save players last location
        pSpawn.MovePlayerOnGrass();

        mapManager.Savelevel();
    }

    public void GenerateWorld()
    {
        if (File.Exists(Application.persistentDataPath + "/map/home_floor.json") && File.Exists(Application.persistentDataPath + "/map/home_walls.json") && File.Exists(Application.persistentDataPath + "/map/home_objects.json"))
        {

            mapManager.LoadLevel();
            //Debug.Log("Home map already exists");

        }
        else 
        {
            for (int y = yOffset; y < height / 2; y++)
            {

                for (int x = xOffset; x < width / 2; x++)
                {

                    float xCoord = xOffset + x / width * scale + seed;
                    float yCoord = yOffset + y / height * scale + seed;

                    float humidity = Mathf.PerlinNoise(xCoord, yCoord);
                    //float temperature = Mathf.PerlinNoise(xCoord, yCoord);
                    float altitude = Mathf.PerlinNoise(xCoord, yCoord);

                    GenerateTile(x, y, humidity, altitude);


                }

            }

            mapManager.Savelevel();

            pSpawn.MovePlayerOnGrass();
            homeSpawn.spawnHome();

            mapManager.Invoke("SaveLevelObjects", 1);
        }

    }

    void GenerateTile(int x, int y, float humidity, float altitude)
    {
        float humidityNum = 0.85f;
        float altitudeNum = 0.75f;

        perlin = Mathf.PerlinNoise(x + Random.value, y + Random.value);

        float random = Random.Range(0f, 1f);

        if (humidity < humidityNum && altitude < altitudeNum)
        {
            //grass
            if (random <= vegetationChance)
            {
                perlin *= forestTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);
                int rand_val = Random.Range(0, forestVegetation.Length);

                tileMap.SetTile(new Vector3Int(x, y, 0), forestTiles[tileNum]);
                GameObject plant = (GameObject)Instantiate(forestVegetation[rand_val], new Vector3(x, y, 0), Quaternion.identity);
                plant.transform.parent = transform;
                plant.layer = 17;

                objectList.Add(forestVegetation[rand_val]);
                posList.Add(new Vector3(x, y, 0));
            }
            else if (random >= 1f - rocksChance)
            {
                perlin *= forestTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);
                int rand_val = Random.Range(0, forestRocks.Length);

                tileMap.SetTile(new Vector3Int(x, y, 0), forestTiles[tileNum]);
                GameObject plant = (GameObject)Instantiate(forestRocks[rand_val], new Vector3(x, y, 0), Quaternion.identity);
                plant.transform.parent = transform;

                plant.layer = 17;
                objectList.Add(forestRocks[rand_val]);
                posList.Add(new Vector3(x, y, 0));
            }
            else
            {
                perlin *= forestTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);

                tileMap.SetTile(new Vector3Int(x, y, 0), forestTiles[tileNum]);
            }

        }
        else if (humidity >= humidityNum && altitude < altitudeNum)
        {
            //water
            perlin *= waterTiles.Length - 1;
            int tileNum = Mathf.RoundToInt(perlin);
            wallsTileMap.SetTile(new Vector3Int(x, y, 0), waterTiles[tileNum]);
        }
        else if (humidity < humidityNum && altitude >= altitudeNum)
        {
            //rock
            tileMap.SetTile(new Vector3Int(x, y, 0), moutainTile);
        }
        else if (humidity >= humidityNum && altitude >= altitudeNum) 
        {
            //mountaine lake
            wallsTileMap.SetTile(new Vector3Int(x, y, 0), mountainWaterTile);
        }

    }

}

class MapObjects 
{
    public List<GameObject> gameObject = new List<GameObject>();
    public List<Vector3> position = new List<Vector3>();

    public MapObjects(List<GameObject> gameObject, List<Vector3> position) 
    {
        this.gameObject = gameObject;
        this.position = position;
    }
}