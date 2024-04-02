using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntitySpawner : MonoBehaviour
{
    public static EntitySpawner Instance { get; private set; }

    [SerializeField] private List<EntitySpawnerSO> locationEntitySpawners = new List<EntitySpawnerSO>();
    [SerializeField] private bool spawnEntities = true;
    [SerializeField] private Vector2 spawnLimits;
    [SerializeField] private Vector2 spawnInternalLimits;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private float maxDistanceFromPlayer;
    [SerializeField] private AStarManager aStarManager;
    [SerializeField] private bool isDynamicLocation;
    [SerializeField] private MapTypeController homeLocation;
    [SerializeField] private MapTypeController dynamicLocation;

    private int spawnCount;
    private bool canSpawn = false;
    private HashSet<Vector2Int> grassTiles;
    private const string TILE_DATA_PATH = "/floor.json";
    private EntitySpawnerSO currentSpawner;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (aStarManager == null)
        {
            AStarManager_OnFinishedScanning();
            return;
        }
        aStarManager.OnFinishedScanning += AStarManager_OnFinishedScanning;
    }

    private void OnDestroy()
    {
        if (aStarManager == null) { return; }
        aStarManager.OnFinishedScanning -= AStarManager_OnFinishedScanning;
    }

    private void AStarManager_OnFinishedScanning()
    {
        canSpawn = true;
        GetTileData();
        ChooseEntitySpawner();
        StartCoroutine(SpawnWithCooldown());
    }

    private void ChooseEntitySpawner()
    {
        if (isDynamicLocation)
        {
            currentSpawner = locationEntitySpawners[dynamicLocation.type - 1];
        } else
        {
            currentSpawner = locationEntitySpawners[0];
        }
    }

    private IEnumerator SpawnWithCooldown()
    {
        while (true)
        {
            if (spawnEntities && canSpawn && spawnCount < maxSpawnCount) { SpawnRandomEntity(); }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnRandomEntity()
    {
        // choosing entity to spawn
        BaseEntity entity;
        if (DaycycleManager.instance != null && DaycycleManager.instance.IsNight)
        {
            entity = currentSpawner.nightEntities[Random.Range(0, currentSpawner.nightEntities.Count)];
        } else
        {
            entity = currentSpawner.dayEntities[Random.Range(0, currentSpawner.dayEntities.Count)];
        }

        SpawnEntity(entity);
    }

    public BaseEntity SpawnEntity(BaseEntity entity, Vector2 position)
    {
        Quaternion rotation = Quaternion.identity;
        BaseEntity instance = Instantiate(entity, position, rotation, transform);

        instance.MaxDistanceFromPlayer = maxDistanceFromPlayer;
        instance.OnDeath += SpawnedEntity_OnDeath;
        instance.OnTooFar += DeleteEntity;
        spawnCount++;
        return instance;
    }

    public BaseEntity SpawnEntity(BaseEntity entity)
    {
        if (GetSafePosition(out Vector2 position))
        {
            return SpawnEntity(entity, position);
        }
        return null;
    }

    public bool GetSafePosition(out Vector2 safePosition)
    {
        Vector2 position = new Vector2();
        Vector2 minBound = new Vector2();
        Vector2 maxBound = new Vector2();
        
        // finding positions of visible corners on camera
        Vector2 cameraLeftBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));
        Vector2 cameraRightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.transform.position.z));
        
        int maxPosChecked = 100;
        bool isAllowedPosition = false;

        while (!isAllowedPosition)
        {
            // choosing zone for spawning
            switch (Random.Range(0, 4))
            {
                // left of the camera
                case 0:
                    minBound = new Vector2(cameraLeftBottom.x - spawnLimits.x,
                                           cameraLeftBottom.y);
                    maxBound = new Vector2(cameraLeftBottom.x - spawnInternalLimits.x,
                                           cameraRightTop.y);
                    break;
                // right of the camera
                case 1:
                    minBound = new Vector2(cameraRightTop.x + spawnInternalLimits.x,
                                           cameraLeftBottom.y);
                    maxBound = new Vector2(cameraRightTop.x + spawnLimits.x,
                                           cameraRightTop.y);
                    break;
                // above the camera
                case 2:
                    minBound = new Vector2(cameraLeftBottom.x,
                                           cameraRightTop.y + spawnInternalLimits.y);
                    maxBound = new Vector2(cameraRightTop.x,
                                           cameraRightTop.y + spawnLimits.y);
                    break;
                // below the camera
                case 3:
                    minBound = new Vector2(cameraLeftBottom.x,
                                           cameraLeftBottom.y - spawnLimits.y);
                    maxBound = new Vector2(cameraRightTop.x,
                                           cameraLeftBottom.y - spawnInternalLimits.y);
                    break;
            }

            // random position within chosen zone
            position = new Vector2(Random.Range(minBound.x, maxBound.x),
                                   Random.Range(minBound.y, maxBound.y));

            isAllowedPosition = IsGround(position);
            maxPosChecked--;

            if (maxPosChecked < 0)
            {
                safePosition = new Vector2();
                return false;
            }
        }

        safePosition = position;
        return true;
    }

    private void GetTileData()
    {
        try
        {
            string path = Path.Join(Application.persistentDataPath, isDynamicLocation?dynamicLocation.directory:homeLocation.directory, TILE_DATA_PATH);
            string json = File.ReadAllText(path);
            SpawnData spawnData = JsonUtility.FromJson<SpawnData>(json);
            grassTiles = new HashSet<Vector2Int>(spawnData.poses.ConvertAll(vec => new Vector2Int(vec.x, vec.y)));
        } catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public bool IsGround(Vector2 position)
    {
        if (grassTiles == null) { return true; }
        return grassTiles.Contains(position.ToVector2Int());
    }

    public bool IsSafePosition(Vector2 position)
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(position);
        if (Mathf.Min(viewportPos.x, viewportPos.y) >= 0 - spawnInternalLimits.y 
            && Mathf.Max(viewportPos.x, viewportPos.y) <= 1 + spawnInternalLimits.y)
        {
            return false;
        }

        return IsGround(position);
    }

    public void DeleteEntity(BaseEntity entity)
    {
        Destroy(entity.gameObject);
        spawnCount--;
    }

    private void SpawnedEntity_OnDeath(BaseEntity entity)
    {
        spawnCount--;
    }
}
