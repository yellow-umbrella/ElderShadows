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

    [SerializeField] private List<BaseEntity> entities = new List<BaseEntity>();
    [SerializeField] private Vector2 spawnLimits;
    [SerializeField] private Vector2 spawnInternalLimits;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private float maxDistanceFromPlayer;
    [SerializeField] private AStarManager aStarManager;

    private int spawnCount;
    private bool canSpawn = false;
    private HashSet<Vector2Int> grassTiles;
    private const string TILE_DATA_PATH = "/map/home/floor.json";

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
        aStarManager.OnFinishedScanning += AStarManager_OnFinishedScanning;
    }

    private void OnDestroy()
    {
        aStarManager.OnFinishedScanning -= AStarManager_OnFinishedScanning;
    }

    private void AStarManager_OnFinishedScanning()
    {
        canSpawn = true;
        GetTileData();
        StartCoroutine(SpawnWithCooldown());
    }

    private IEnumerator SpawnWithCooldown()
    {
        while (true)
        {
            if (canSpawn && spawnCount < maxSpawnCount) { SpawnRandomEntity(); }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnRandomEntity()
    {
        // choosing entity to spawn
        BaseEntity entity = entities[Random.Range(0, entities.Count)];

        //find safe position to spawn
        Vector2 position = new Vector2();
        if (!GetSafePosition(spawnLimits, ref position))
        {
            return;
        }

        SpawnEntity(entity, position);
    }

    public void SpawnEntity(BaseEntity entity, Vector2 position)
    {
        Quaternion rotation = Quaternion.identity;
        BaseEntity instance = Instantiate(entity, position, rotation, transform);

        instance.MaxDistanceFromPlayer = maxDistanceFromPlayer;
        instance.OnDeath += SpawnedEntity_OnDeath;
        instance.OnTooFar += SpawnedEntity_OnTooFar;
        spawnCount++;
    }

    public bool GetSafePosition(Vector2 spawnLimits, ref Vector2 safePosition)
    {
        bool isAllowedPosition = false;
        Vector2 position = new Vector2();
        Vector2 cameraLeftBottom = new Vector2();
        Vector2 cameraRightTop = new Vector2();
        Vector2 minBound = new Vector2();
        Vector2 maxBound = new Vector2();
        int maxPosChecked = 100;

        while (!isAllowedPosition)
        {
            // finding positions of visible corners on camera
            cameraLeftBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));
            cameraRightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.transform.position.z));

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

            if (maxPosChecked <= 0)
            {
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
            string json = File.ReadAllText(Application.persistentDataPath + TILE_DATA_PATH);
            SpawnData spawnData = JsonUtility.FromJson<SpawnData>(json);
            grassTiles = new HashSet<Vector2Int>(spawnData.poses.ConvertAll(vec => new Vector2Int(vec.x, vec.y)));
        } catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public bool IsGround(Vector2 position)
    {
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

    private void SpawnedEntity_OnTooFar(BaseEntity entity)
    {
        Destroy(entity.gameObject);
        spawnCount--;
    }

    private void SpawnedEntity_OnDeath(BaseEntity entity)
    {
        spawnCount--;
    }
}
