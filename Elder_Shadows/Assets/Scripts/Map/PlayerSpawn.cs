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
    private int AssertPosition;
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

        if(data.poses.Count > 0)
        {
            AssertPosition = (int)(data.poses.Count / 2);
            spawn_position = data.poses[AssertPosition];
        }

        player.transform.position = spawn_position;

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
}

public class SpawnData
{
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> poses = new List<Vector3Int>();
}
