using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeSelectionUIManager : BaseUIManager
{
    [SerializeField] private SceneReference previousScene;

    public MapTypeController mapType;

    public void LoadPreviousScene()
    {
        LoadScene(previousScene);
    }

    public void ChooseLandscapeA(int input)
    {
        //apply landscape A
        mapType.type = input;
        Debug.Log("Landscape Forest");
        LoadNextScene();
    }

    public void ChooseLandscapeB(int input)
    {
        //apply landscape B
        mapType.type = input;
        Debug.Log("Landscape Field");
        LoadNextScene();
    }

    public void ChooseLandscapeC(int input)
    {
        //apply landscape C
        mapType.type = input;
        Debug.Log("Landscape Mountains");
        LoadNextScene();
    }
}
