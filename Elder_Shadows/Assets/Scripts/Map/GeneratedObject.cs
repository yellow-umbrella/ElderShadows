using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedObject : MonoBehaviour
{
    [SerializeField] private int SecondsUntilDisappear;
    private void Start()
    {
        StartCoroutine(Rot());
    }

    IEnumerator Rot()
    {
        yield return new WaitForSeconds(SecondsUntilDisappear);
        DestroyFruit();
    }

    public void PickUpFruit()
    {
        DestroyFruit();
    }

    void DestroyFruit()
    {
        gameObject.GetComponentInParent<GenerativeObject>().GeneratedFruits.Remove(gameObject);
        Destroy(gameObject);    
    }
}
