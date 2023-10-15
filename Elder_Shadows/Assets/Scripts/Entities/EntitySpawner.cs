using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private List<BaseEntity> entities = new List<BaseEntity>();
    [SerializeField] private Vector2 spawnLimits;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int maxSpawnCount;

    private int spawnCount;
    private float currentSpawnDelay;

    private void Update()
    {
        currentSpawnDelay -= Time.deltaTime;
        if (currentSpawnDelay <= 0)
        {
            currentSpawnDelay = spawnDelay;
            if (spawnCount < maxSpawnCount)
            {
                SpawnRandomEntity();
            }
        }
    }

    private void SpawnRandomEntity()
    {
        BaseEntity entity = entities[Random.Range(0, entities.Count)];
        Vector2 position = new Vector2(Random.Range(-spawnLimits.x, spawnLimits.x),
                            Random.Range(-spawnLimits.y, spawnLimits.y));
        Quaternion rotation = Quaternion.identity;
        BaseEntity instance = Instantiate(entity, position, rotation, transform);
        
        instance.OnDeath += SpawnedEntity_OnDeath;
        spawnCount++;
    }

    private void SpawnedEntity_OnDeath(object sender, System.EventArgs e)
    {
        spawnCount--;
    }
}
