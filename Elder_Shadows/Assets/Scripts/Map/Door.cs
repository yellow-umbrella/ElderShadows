using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    public int nextLocationIdx;

    public void GoToLocation(int index) 
    {
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    public void Interact() 
    {
        GoToLocation(nextLocationIdx);
    }
}
