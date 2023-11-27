using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HomeSpawn : MonoBehaviour
{
    public GameObject home;
    public GameObject player;

    public void spawnHome()
    {
        Vector3 home_position = new Vector3(player.transform.position.x, player.transform.position.y + 1, 0);



        GameObject homeObj = (GameObject)Instantiate(home, home_position, Quaternion.identity);
        homeObj.transform.parent = transform;
        homeObj.layer = 3;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
