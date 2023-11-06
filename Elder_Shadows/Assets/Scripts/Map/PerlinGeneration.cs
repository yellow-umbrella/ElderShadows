using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class PerlinGeneration : MonoBehaviour
{

    public Tilemap tileMap;
    public Tilemap wallsTileMap; 
    public TileBase[] forestTiles;
    public TileBase[] desertTiles;
    public GameObject[] forestVegetation;
    //public GameObject[] desertVegetation;

    public float perlin;
    public float scale;
    public float seed;

    public int xOffset;
    public int yOffset;

    public float width;
    public float height;

    public float vegetationChance;

    private void Start()
    {

        seed = Random.Range(0f, 100000f);
        generateWorld();

    }

    public void generateWorld()
    {

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

            vegetationChance = 0.0175f;

            if (random <= vegetationChance)
            {

                perlin *= forestVegetation.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);

                //wallsTileMap.SetTile(new Vector3Int(x, y, 0), forestVegetation[tileNum]);
                //tileMap.SetTile(new Vector3Int(x, y, 0), forestVegetation[tileNum]); 
                tileMap.SetTile(new Vector3Int(x, y, 0), forestTiles[tileNum]);
                GameObject plant = Instantiate(forestVegetation[0], new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                //GameObject plant = Instantiate(forestVegetation[Random.Range(0, forestVegetation.Length)], new Vector3(x + 0.5f, y + 5.15f, 0), Quaternion.identity);
                //plant.transform.parent = gameObject.transform.GetChild(0);
                plant.layer = 3;
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

                perlin *= desertTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);

                //tileMap.SetTile(new Vector3Int(x, y, 0), desertTiles[tileNum]);
                wallsTileMap.SetTile(new Vector3Int(x, y, 0), desertTiles[tileNum]);
            }
            else
            {

                perlin *= desertTiles.Length - 1;
                int tileNum = Mathf.RoundToInt(perlin);

                wallsTileMap.SetTile(new Vector3Int(x, y, 0), desertTiles[tileNum]);
                //tileMap.SetTile(new Vector3Int(x, y, 0), desertTiles[tileNum]);

            }

        }

    }

}