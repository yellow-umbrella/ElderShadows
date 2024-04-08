using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectQuestStep : QuestStep
{
    [SerializeField] ItemObject itemToCollect;
    [SerializeField] int amountToCollect;
    [SerializeField] private bool needToSpawn = false;
    [SerializeField] private EntitySpawner.MapType placeToSpawn;
    [SerializeField] private GroundItem itemPrefab;

    int amountCollected = 0;
    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Awake()
    {
        QuestManager.instance.onTryFinishQuest += CheckCollectedItems;
    }

    private void Start()
    {
        if (EntitySpawner.Instance != null)
        {
            EntitySpawner.Instance.OnStartSpawning += SpawnItems;
        }
    }

    private void SpawnItems()
    {
        if (needToSpawn && placeToSpawn == EntitySpawner.Instance.CurrentLocation)
        {
            amountCollected = CharacterController.instance.inventory.GetAmountOfItem(new Item(itemToCollect));
            for (int i = 0; i < amountToCollect - amountCollected; i++)
            {
                Vector2 pos = EntitySpawner.Instance.GetGlobalSafePosition();
                GroundItem groundItem = Instantiate(itemPrefab, pos, Quaternion.identity, transform);
                groundItem.gameObject.GetComponent<SpriteRenderer>().sprite = itemToCollect.uiDisplay;
                groundItem.item = itemToCollect;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        QuestManager.instance.onTryFinishQuest -= CheckCollectedItems;
        if (EntitySpawner.Instance != null)
        {
            EntitySpawner.Instance.OnStartSpawning -= SpawnItems;
        }
    }

    private void CheckCollectedItems(string id)
    {
        if (id != questId) return;
        Item item = new Item(itemToCollect);
        Debug.Log("Checking collected items: " + CharacterController.instance.inventory.GetAmountOfItem(item));
        if (CharacterController.instance.inventory.RemoveItem(item, amountToCollect))
        {
            FinishQuestStep();        
        }
    }

    private void UpdateState()
    {
        amountCollected = CharacterController.instance.inventory.GetAmountOfItem(new Item(itemToCollect));
        string state = amountCollected.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.amountCollected = System.Int32.Parse(state);
        UpdateState();
    }

    public void SetData(ItemObject item, int amount)
    {
        itemToCollect = item;
        amountToCollect = amount;
    }
}
