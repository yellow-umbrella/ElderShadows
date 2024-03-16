using MyBox;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUIManager : MonoBehaviour
{
    public SceneReference scene;
    public MapTypeController mapTypeController;

    public void GoToLocation() 
    {
        SceneManager.LoadScene(scene.SceneName);
    }

    public void SetLocationType(int input) 
    {
        mapTypeController.type = input;
    }

    public void SetLocationDirectory(string input) 
    {
        mapTypeController.directory = input;
    }
}
