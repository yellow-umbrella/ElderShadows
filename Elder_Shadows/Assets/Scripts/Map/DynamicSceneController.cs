using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DynamicSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit() 
    {
        DeleteDirectory();
    }

    private void DeleteDirectory()
    {
        string scene_path = Application.persistentDataPath + "/map/dynamic";
        try
        {
            Directory.Delete(scene_path);
        }
        catch
        {
            Debug.LogWarning("Dynamic scene directory does not exist");
        }
    }
}
