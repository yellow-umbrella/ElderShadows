using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeSelectionUIManager : BaseUIManager
{
    [SerializeField] private SceneReference previousScene;

    public void LoadPreviousScene()
    {
        LoadScene(previousScene);
    }

    public void ChooseLandscapeA()
    {
        //apply landscape A
        Debug.Log("Landscape A");
        LoadNextScene();
    }

    public void ChooseLandscapeB()
    {
        //apply landscape B
        Debug.Log("Landscape B");
        LoadNextScene();
    }

    public void ChooseLandscapeC()
    {
        //apply landscape C
        Debug.Log("Landscape C");
        LoadNextScene();
    }
}
