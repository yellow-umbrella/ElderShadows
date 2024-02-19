using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    public int nextLocationIdx;

    protected bool isInteracting = true;
    protected bool canInteract = true;

    public void GoToLocation(int index) 
    {
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    public virtual void Interact() 
    {
        GoToLocation(nextLocationIdx);
    }

    public virtual bool CanInteract() 
    {
        return canInteract;
    }
    public virtual bool IsInteracting() 
    {
        return isInteracting;
    }
}
