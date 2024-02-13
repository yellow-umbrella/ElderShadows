using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersHouse : MonoBehaviour, IInteractable
{
    public void EnterHouse() 
    {
        SceneManager.LoadScene("PlayersHouse");
    }

    public void ExitHouse() 
    {
        SceneManager.LoadScene("HomeLocation");
    }

    public void Interact() 
    {
        if (gameObject.layer == 17) 
        {
            EnterHouse();
        }else if (gameObject.layer == 3) 
        {
            ExitHouse();
        }
        
    }
}
