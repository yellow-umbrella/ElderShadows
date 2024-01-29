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
    private int AssertPosition;

    public void MovePlayerOnGrass()
    {
        Vector3Int spawn_position = new Vector3Int();
        string json = File.ReadAllText(Application.persistentDataPath + "/map/home_floor.json");
        
        //file with grass tiles
        SpawnData data = JsonUtility.FromJson<SpawnData>(json);

        if(data.poses.Count > 0)
        {
            AssertPosition = (int)(data.poses.Count / 2);
            spawn_position = data.poses[AssertPosition];
        }

        player.transform.position = spawn_position;

    }
}
public class SpawnData
{
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> poses = new List<Vector3Int>();
}
