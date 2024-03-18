using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerativeObject : MonoBehaviour
{
    public GameObject PlayerCharacter;
    [SerializeField] private int PlayerRange;
    [SerializeField] private GameObject FruitPrefab;
    [SerializeField] private Vector2Int GenerationDelayRange;
    [SerializeField] private Vector2 GeneratableArea;

    public List<GameObject> GeneratedFruits = new List<GameObject>();

    private bool playerInReach = false;
    private bool playerWasInReach = false;

    private void Start()
    {
        StartCoroutine(GenerateFruit());
    }

    private void FixedUpdate()
    {
        if (playerInReach && !playerWasInReach)
        {
            playerWasInReach = true;
        }
        else if (!playerInReach && playerWasInReach)
        {
            playerWasInReach = false;
            foreach (var fruit in GeneratedFruits)
            {
                fruit.SetActive(false);
            }
        }
        
        if (Vector2.Distance(PlayerCharacter.transform.position, transform.position) < PlayerRange && !playerWasInReach)
        {
            playerInReach = true;
            foreach (var fruit in GeneratedFruits)
            {
                fruit.SetActive(true);
            }
        }
        else if (Vector2.Distance(PlayerCharacter.transform.position, transform.position) > PlayerRange && playerWasInReach)
        {
            playerInReach = false;
        }
    }

    IEnumerator GenerateFruit()
    {
        while (true)
        {
            if (GeneratedFruits.Count < 3)
            {
                float time = Random.Range(GenerationDelayRange.x, GenerationDelayRange.y);

                yield return new WaitForSeconds(time);
                var fruit = CreateFruit();
                GeneratedFruits.Add(fruit);
                fruit.GetComponent<GeneratedObject>().ParentTree = this;

                if (!playerInReach)
                    fruit.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(10f);
            }
        }
    }

    private GameObject CreateFruit()
    {
        GameObject newFruit = Instantiate(FruitPrefab, Vector3.zero, Quaternion.identity);
        newFruit.transform.SetParent(transform.parent, false);

        Vector3 position = new Vector3(Random.Range(-GeneratableArea.x, GeneratableArea.x), Random.Range(-GeneratableArea.y, GeneratableArea.y), 0);
        newFruit.transform.position = transform.position + position;
        return newFruit;
    }
}
