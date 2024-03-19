using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Linq;
using System.IO;

public class MapGeneration : MonoBehaviour
{
    public Tilemap tileMap;
    public Tilemap wallsTileMap1;
    public Tilemap wallsTileMap2; 
    public Tilemap wallsTileMap3;

    public MapTypeController mapTypeController;
    public List<MapType> mapType = new List<MapType>();

    private TileBase[] forestTiles;
    private TileBase[] waterTiles;
    private TileBase[] moutainTile;

    private GameObject[] forestVegetation;
    private GameObject[] forestRocks;

    public GameObject PlayerCharacter;

    //public float perlin;
    public float scale;
    private float perlin;
    private float seed;

    public int xOffset;
    public int yOffset;

    public float width;
    public float height;

    private float humidityNum;
    private float altitudeNum;

    private float vegetationChance;
    private float rocksChance;
    
    private List<GameObject> objectList = new List<GameObject>();
    private List<Vector3> posList = new List<Vector3>();
    private MapManager mapManager;
    private PlayerSpawn pSpawn;
    private HomeSpawn homeSpawn;
    private MapObject mapObject;

    private void Awake() 
    {
        mapManager = GetComponent<MapManager>();
        mapObject = GetComponent<MapObject>();
        pSpawn = GetComponent<PlayerSpawn>();
        homeSpawn = GetComponent<HomeSpawn>();

        ChooseMapType();
    }

    private void Start()
    {
        seed = Random.Range(0f, 100000f);
        GenerateWorld();

        // add code to save players last location
        //pSpawn.MovePlayerOnGrass();

        //mapManager.Savelevel();
    }

    private void ChooseMapType() 
    {
        switch (mapTypeController.type) 
        {
            case 0:
                SetMapType(mapType[0]);
                break;
            case 1:
                SetMapType(mapType[1]);
                break;
            case 2:
                SetMapType(mapType[2]);
                break;
            case 3:
                SetMapType(mapType[3]);
                break;
        }
    }

    private void SetMapType(MapType mapType) 
    {
        humidityNum = mapType.humidityNum;
        altitudeNum = mapType.altitudeNum;

        vegetationChance = mapType.vegetationChance;
        rocksChance = mapType.rocksChance;

        forestTiles = mapType.forestTiles;
        waterTiles = mapType.waterTiles;
        moutainTile = mapType.moutainTile;

        forestVegetation = mapType.forestVegetation;
        forestRocks = mapType.forestRocks;
}

    public void GenerateWorld()
    {
        if (File.Exists(Application.persistentDataPath + mapTypeController.directory + "/floor.json") && File.Exists(Application.persistentDataPath + mapTypeController.directory + "/walls.json") && File.Exists(Application.persistentDataPath + mapTypeController.directory + "/objects.json"))
        {

            mapManager.LoadLevel();
            // temporary
            pSpawn.MovePlayerOnGrass();
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

                    float xCoord_a = xOffset + x / width * scale + seed+100000f;
                    float yCoord_a = yOffset + y / height * scale + seed+100000f;

                    float humidity = Mathf.PerlinNoise(xCoord, yCoord);
                    //float temperature = Mathf.PerlinNoise(xCoord, yCoord);
                    float altitude = Mathf.PerlinNoise(xCoord_a, yCoord_a);

                    GenerateTile(x, y, humidity, altitude);


                }

            }

            mapManager.Savelevel();

            pSpawn.MovePlayerOnGrass();
            //homeSpawn.spawnHome();

            //mapManager.Invoke("SaveLevelObjects", 1);
        }

    }

    void GenerateTile(int x, int y, float humidity, float altitude)
    {
        //float humidityNum = 0.65f;
        //float altitudeNum = 0.75f;

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
                
                if (plant.GetComponent<GenerativeObject>() != null)
                {
                    plant.GetComponent<GenerativeObject>().PlayerCharacter = PlayerCharacter;
                }
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
        else if (humidity < humidityNum && altitude >= altitudeNum || humidity > humidityNum && altitude >= altitudeNum)
        {
            float dif = altitude - altitudeNum;
            //rock
            if (0.025 <= dif)
            {
                wallsTileMap2.SetTile(new Vector3Int(x, y, 0), moutainTile[2]);
            }
            if (0.05 < dif)
            {
                wallsTileMap3.SetTile(new Vector3Int(x, y, 0), moutainTile[1]);
            }
            if (0 < dif)
            {
                wallsTileMap1.SetTile(new Vector3Int(x, y, 0), moutainTile[0]);
            }

            perlin *= forestTiles.Length - 1;
            int tileNum = Mathf.RoundToInt(perlin);
            tileMap.SetTile(new Vector3Int(x, y, 0), forestTiles[tileNum]);
        }
        else if (humidity >= humidityNum && altitude < altitudeNum)
        {
            //water
            perlin *= waterTiles.Length - 1;
            int tileNum = Mathf.RoundToInt(perlin);
            wallsTileMap1.SetTile(new Vector3Int(x, y, 0), waterTiles[tileNum]);
        }
        /*else if (humidity >= humidityNum && altitude >= altitudeNum) 
        {
            //mountaine lake
            wallsTileMap.SetTile(new Vector3Int(x, y, 0), mountainWaterTile);
        }*/
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