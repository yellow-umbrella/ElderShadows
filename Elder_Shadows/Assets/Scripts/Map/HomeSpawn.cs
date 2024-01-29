using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HomeSpawn : MonoBehaviour
{
    public GameObject home;
    public GameObject player;
    public LayerMask treesLayer;

    private MapManager mapManager = new MapManager();

    //

    public void spawnHome()
    {
        Vector3 home_position = new Vector3(player.transform.position.x, player.transform.position.y + 1, 0);

        GameObject homeObj = (GameObject)Instantiate(home, home_position, Quaternion.identity);
        homeObj.transform.parent = transform;
        homeObj.layer = 17;

        removeTrees();
    }

    public void removeTrees()
    {
        Collider2D[] tree_colliders = Physics2D.OverlapCircleAll(player.transform.position, 4f, treesLayer);
        //List<GameObject> treesList = new List<GameObject>();

        foreach (Collider2D c in tree_colliders)
        {
            if(c.gameObject.tag == "Tree") 
            {
                //treesList.Add(c.gameObject);
                //mapManager.DeleteLevelObject(c.gameObject);

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
