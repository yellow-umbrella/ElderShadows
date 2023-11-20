using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private List<BaseEntity> entities = new List<BaseEntity>();
    [SerializeField] private Vector2 spawnLimits;
    [SerializeField] private Vector2 spawnInternalLimits;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private AStarManager aStarManager;

    private int spawnCount;
    private float currentSpawnDelay;
    private bool canSpawn = false;

    private void Start()
    {
        aStarManager.OnFinishedScanning += AStarManager_OnFinishedScanning;
    }

    private void OnDestroy()
    {
        aStarManager.OnFinishedScanning -= AStarManager_OnFinishedScanning;
    }

    private void AStarManager_OnFinishedScanning()
    {
        canSpawn = true;
    }

    private void Update()
    {
        if (canSpawn)
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
    }

    private void SpawnRandomEntity()
    {
        BaseEntity entity = entities[Random.Range(0, entities.Count)];
        bool isAllowedPosition = false;
        Vector2 position = new Vector2();
        Vector2 onCameraPosition = new Vector2();
        int maxPosChecked = 1000;
        while (!isAllowedPosition)
        {
            position = new Vector2(Random.Range(-spawnLimits.x, spawnLimits.x),
                                Random.Range(-spawnLimits.y, spawnLimits.y));
            onCameraPosition = Camera.main.WorldToViewportPoint(position);
            isAllowedPosition = !(0 <= onCameraPosition.x && onCameraPosition.x <= 1
                && 0 <= onCameraPosition.y && onCameraPosition.y <= 1); // TODO check also for obstacles
            maxPosChecked--;
            if (maxPosChecked <= 0)
            {
                return;
            }
        }
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
