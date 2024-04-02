using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectQuestStep : QuestStep
{
    [SerializeField] ItemObject itemToCollect;
    [SerializeField] int amountToCollect;

    int amountCollected = 0;

    private void Awake()
    {
        QuestManager.instance.onTryFinishQuest += CheckCollectedItems;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        QuestManager.instance.onTryFinishQuest -= CheckCollectedItems;
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
