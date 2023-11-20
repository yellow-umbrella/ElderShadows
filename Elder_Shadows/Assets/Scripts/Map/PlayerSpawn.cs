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
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovePlayerOnGrass()
    {
        Vector3Int new_position = new Vector3Int();
        string json = File.ReadAllText(Application.dataPath + "/homeLevel.json");
        string c_json = File.ReadAllText(Application.dataPath + "/homeCollisionsLevel.json");
        SpawnData data = JsonUtility.FromJson<SpawnData>(json);
        SpawnData c_data = JsonUtility.FromJson<SpawnData>(c_json);

        if(data.poses.Count > 0)
        {
            AssertPosition = (int)(data.poses.Count / 2);
            new_position = data.poses[AssertPosition];
        }

        player.transform.position = new_position;

    }
}
public class SpawnData
{
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> poses = new List<Vector3Int>();
}
