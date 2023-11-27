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

    public void ChooseLandscapeA(string input)
    {
        //apply landscape A
        MapTypeController.landscapeType = input;
        Debug.Log("Landscape A");
        LoadNextScene();
    }

    public void ChooseLandscapeB(string input)
    {
        //apply landscape B
        MapTypeController.landscapeType = input;
        Debug.Log("Landscape B");
        LoadNextScene();
    }

    public void ChooseLandscapeC(string input)
    {
        //apply landscape C
        MapTypeController.landscapeType = input;
        Debug.Log("Landscape C");
        LoadNextScene();
    }
}
