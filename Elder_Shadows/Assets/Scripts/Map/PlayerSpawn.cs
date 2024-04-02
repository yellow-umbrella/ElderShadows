using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Linq;
using System.IO;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;
    public MapTypeController mapTypeController;
    private List<Vector3Int> AssertPosition = new List<Vector3Int>();
    private MapManager mapManager;

    private void Awake()
    {
        mapManager = GetComponent<MapManager>();
    }

    public void Start() 
    {

        //Invoke("MovePlayerOnGrass", 0.1f);

    }

    public void MovePlayerOnGrass()
    {
        //mapManager.Savelevel();
        Vector3Int spawn_position = new Vector3Int();
        string json = File.ReadAllText(Application.persistentDataPath + mapTypeController.directory + "/floor.json");
        
        //file with grass tiles
        SpawnData data = JsonUtility.FromJson<SpawnData>(json);
        Vector3Int lag = new Vector3Int(40, 30, 0);
        Vector3Int centre = findFloorCentre();

        /*if (data.poses.Count > 0) 
        {
            player.transform.position = data.poses[data.poses.Count / 2];
            Debug.Log((centre.x - lag.x)+" !!!!!!! "+ centre);
        }*/
        
        try
        {
            if (data.poses.Count > 0)
            {
                foreach (var pose in data.poses)
                {
                    float centerDistance = Vector3Int.Distance(pose, centre);

                    if (centerDistance < 30) 
                    {
                        AssertPosition.Add(pose);
                    }
                }

                spawn_position = AssertPosition[Random.Range(0, AssertPosition.Count-1)];
            }

            player.transform.position = spawn_position;
        }
        catch 
        {
            Debug.Log("MovePlayerOnGrass method in PlayerSpawn.cs not working");
        }
    }

    public void isPlayerOnGrass() 
    {
        string json = File.ReadAllText(Application.persistentDataPath + mapTypeController.directory + "/floor.json");

        //file with grass tiles
        SpawnData data = JsonUtility.FromJson<SpawnData>(json);

        foreach(var pose in data.poses)
        {
            if(pose == player.transform.position) 
            {
                Debug.Log("Player is on grass");
            }
        }
    }

    public Vector3Int findFloorCentre() 
    {
        string json = File.ReadAllText(Application.persistentDataPath + mapTypeController.directory + "/floor.json");

        //file with grass tiles
        SpawnData data = JsonUtility.FromJson<SpawnData>(json);

        Vector3Int minVector = data.poses[0];
        Vector3Int maxVector = data.poses[0];

        foreach (var vector in data.poses)
        {
            if (vector.x <= minVector.x && vector.y <= minVector.y)
            {
                minVector = vector;
            }
        }

        foreach (var vector in data.poses)
        {
            if (vector.x >= minVector.x && vector.y >= minVector.y)
            {
                maxVector = vector;
            }
        }

        Vector3Int resVector = (minVector + maxVector)/2;

        return resVector;
    }
}

public class SpawnData
{
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> poses = new List<Vector3Int>();
}
