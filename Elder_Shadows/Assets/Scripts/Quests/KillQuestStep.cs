using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillQuestStep : QuestStep
{
    [SerializeField] EntityInfoSO entityToKill;
    [SerializeField] int amountToKill;

    int amountKilled = 0;

    private void Update()
    {
        // should be called when entity is killed
        CheckKilledEntity("");
    }

    private void CheckKilledEntity(string id)
    {
        if (entityToKill.id == id)
        {
            amountKilled++;
        }

        if (amountKilled >= amountToKill)
        {
            FinishQuestStep();
        }
    }
}
