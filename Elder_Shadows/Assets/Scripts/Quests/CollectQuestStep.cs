using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectQuestStep : QuestStep
{
    [SerializeField] ItemObject itemToCollect;
    [SerializeField] int amountToCollect;

    int amountCollected = 0;

    private void Update()
    {
        // should be called when inventar changes
        CheckCollectedItems();
    }

    private void CheckCollectedItems()
    {
        // TODO check what amount of this item has player
        // amountCollected = Player.instance.ItemCount(itemToCollect.id);
        // UpdateState()
        if (amountCollected >= amountToCollect)
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
}
