using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMask : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject shadow;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Entering tree collider mask");
        if(other.gameObject.tag == "Character")
            transform.parent.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f);
    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Character")
            transform.parent.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    private void FixedUpdate()
    {
        shadow.SetActive(!IsOffScreen());
    }
    
    bool IsOffScreen() {
        var bounds = renderer.bounds;

        var top = camera.WorldToViewportPoint(bounds.max);
        var bottom = camera.WorldToViewportPoint(bounds.min);

        return top.y < -0.5 || bottom.y > 1.5 || top.x > 1.5 || bottom.x < -0.5;    
    }
}
