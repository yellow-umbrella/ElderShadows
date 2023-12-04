using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMask : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entering tree collider mask");
        if(other.gameObject.tag == "Character")
            transform.parent.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f);
    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Character")
            transform.parent.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
