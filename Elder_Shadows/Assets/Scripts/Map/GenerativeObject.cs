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

                var fruit = CreateFruit();
                GeneratedFruits.Add(fruit);

                if (!playerInReach)
                    fruit.SetActive(false);

                yield return new WaitForSeconds(time);
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
        newFruit.transform.SetParent(transform, false);

        Vector2 position = new Vector2(Random.Range(-GeneratableArea.x, GeneratableArea.x), Random.Range(-GeneratableArea.y, GeneratableArea.y));
        newFruit.transform.localPosition = position;
        return newFruit;
    }
}
