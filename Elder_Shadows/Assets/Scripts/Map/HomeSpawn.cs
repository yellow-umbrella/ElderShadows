using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class HomeSpawn : MonoBehaviour
{
    public GameObject home;
    public GameObject player;
    public LayerMask treesLayer;

    private MapManager mapManager;

    private void Awake()
    {
        mapManager = GetComponent<MapManager>();
    }

    private void Start() 
    {
        spawnHome();
        //Invoke("spawnHome()", 0.5f);
    }

    public void spawnHome()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/map/home/objects.json");
        LevelObject levelObjects =  JsonUtility.FromJson<LevelObject>(json);

        if (!levelObjects.objects.Contains("house1")) 
        {
            Vector3 home_position = new Vector3(player.transform.position.x, player.transform.position.y + 1, 0);

            GameObject homeObj = (GameObject)Instantiate(home, home_position, Quaternion.identity);
            homeObj.transform.parent = transform;
            homeObj.layer = 17;

            removeTrees();
            mapManager.Invoke("Savelevel", 1);
        }

        //mapManager.Savelevel();

        
    }

    public void removeTrees()
    {
        Collider2D[] tree_colliders = Physics2D.OverlapCircleAll(player.transform.position, 4f, treesLayer);

        foreach (Collider2D c in tree_colliders)
        {
            if(c.gameObject.tag == "Tree") 
            {
                Debug.Log("Tree" + c.transform.position);
                Destroy(c.gameObject);
            }
            
            
        }
    }

    public Vector3 getHomePosition()
    {
        return home.transform.position;
    }
}
