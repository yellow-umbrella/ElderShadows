using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersHouse : MonoBehaviour, IInteractable
{
    //move other house functions there

    public void EnterHouse() 
    {
        SceneManager.LoadScene(4, LoadSceneMode.Single);
    }

    public void Interact() 
    {
        EnterHouse();
    }
}
