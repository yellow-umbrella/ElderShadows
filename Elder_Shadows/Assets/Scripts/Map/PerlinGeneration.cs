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
    public GameObject[] forestVegetation;
    public GameObject[] forestRocks;
    
    //public GameObject[] desertVegetation;

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
    //private MapManager mManager;

    private void Start()
    {

        seed = Random.Range(0f, 100000f);
        generateWorld();

        saveObjects();

    }
    private void Update() 
    {
        
    }

    public void generateWorld()
    {
        if (File.Exists(Application.dataPath + "/homeObjects.json") && File.Exists(Application.dataPath + "/homeLevel.json") && File.Exists(Application.dataPath + "/homeCollisionsLevel.json"))
        {
            Debug.Log("Home map already exists");

            /*GameObject mapManager = new GameObject("MapManager");
            mManager = mapManager.AddComponent<MapManager>();

            mManager.LoadLevel();*/

        }
        
        for (int y = yOffset; y < height / 2; y++)
        {

            for (int x = xOffset; x < width / 2; x++)
            {

                float xCoord = xOffset + x / width * scale + seed;
                float yCoord = yOffset + y / height * scale + seed;

                float rainFall = Mathf.PerlinNoise(xCoord, yCoord);
                float temperature = Mathf.PerlinNoise(xCoord, yCoord);

                generateTile(x, y, temperature, rainFall);


            }

        }

    }

    void generateTile(int x, int y, float temperature, float rainFall)
    {

        //Debug.Log("Generating Tile");

        perlin = Mathf.PerlinNoise(x + Random.value, y + Random.value);

        float random = Random.Range(0f, 1f);

        if (temperature <= 0.75f && rainFall >= 0.25f || temperature > 0.75f && rainFall >= 0.25f)
        {

            vegetationChance = 0.05f;
            rocksChance = 0.01f;

            if (random <= vegetationChance)
            {
                
                perlin *= forestTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);
                int rand_val = Random.Range(0, forestVegetation.Length);



                tileMap.SetTile(new Vector3Int(x, y, 0), forestTiles[tileNum]);
                GameObject plant = Instantiate(forestVegetation[rand_val], new Vector3(x + 0.5f, y + 1.75f, 0), Quaternion.identity);
                
                plant.layer = 3;

                objectList.Add(forestVegetation[rand_val]);
                posList.Add(new Vector3(x + 0.5f, y + 1.75f, 0));
                
            }
            if (random <= 0.062f && random > 0.06f)
            {
                //perlin *= forestVegetation.Length - 1;
                perlin *= forestTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);
                int rand_val = Random.Range(0, forestRocks.Length);



                tileMap.SetTile(new Vector3Int(x, y, 0), forestTiles[tileNum]);
                GameObject plant = Instantiate(forestRocks[rand_val], new Vector3(x + 0.5f, y + 1.75f, 0), Quaternion.identity);
                
                plant.layer = 3;
                objectList.Add(forestRocks[rand_val]);
                posList.Add(new Vector3(x + 0.5f, y + 1.75f, 0));
            }
            else
            {

                perlin *= forestTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);

                tileMap.SetTile(new Vector3Int(x, y, 0), forestTiles[tileNum]);

            }

        }
        else if (temperature > 0.75f && rainFall < 0.25f || temperature <= 0.75f && rainFall < 0.25f)
        {

            vegetationChance = 0.015f;

            if (random <= vegetationChance)
            {

                perlin *= waterTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);

                
                wallsTileMap.SetTile(new Vector3Int(x, y, 0), waterTiles[tileNum]);
            }
            else
            {

                perlin *= waterTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);

                wallsTileMap.SetTile(new Vector3Int(x, y, 0), waterTiles[tileNum]);
                

            }

        }

    }

    public void saveObjects() 
    {
        
        if (!File.Exists(Application.dataPath + "/homeObjects.json"))
        {
            MapObjects mapObjects = new MapObjects(objectList, posList);

            string json = JsonUtility.ToJson(mapObjects, true);
            File.WriteAllText(Application.dataPath + "/homeObjects.json", json);
            //Debug.Log("Objects were saved");
        }
        else
        {
            //Debug.Log("Objects were not saved");
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