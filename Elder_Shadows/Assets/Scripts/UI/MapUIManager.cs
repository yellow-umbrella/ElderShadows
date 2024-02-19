using MyBox;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUIManager : MonoBehaviour
{
    public SceneReference scene;

    public void GoToLocation() 
    {
        SceneManager.LoadScene(scene.SceneName);
    }
}
