using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedObject : MonoBehaviour
{
    [SerializeField] private int SecondsUntilDisappear;
    public GenerativeObject ParentTree;
    private void Start()
    {
        StartCoroutine(Rot());
    }

    IEnumerator Rot()
    {
        yield return new WaitForSeconds(SecondsUntilDisappear);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ParentTree.GeneratedFruits.Remove(gameObject);
    }
}
