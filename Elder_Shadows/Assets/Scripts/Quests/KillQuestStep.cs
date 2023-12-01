using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillQuestStep : QuestStep
{
    [SerializeField] EntityInfoSO entityToKill;
    [SerializeField] int amountToKill;

    int amountKilled = 0;

    private void Start()
    {
        CharacterController.instance.combat.onEnemyKilled.AddListener(CheckKilledEntity);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CharacterController.instance.combat.onEnemyKilled.RemoveListener(CheckKilledEntity);
    }

    private void CheckKilledEntity(string id)
    {
        if (entityToKill.id == id)
        {
            amountKilled++;
            UpdateState();
        }

        if (amountKilled >= amountToKill)
        {
            FinishQuestStep();
        }
    }

    [ButtonMethod]
    private void Kill()
    {
        CheckKilledEntity(entityToKill.id);
    }

    private void UpdateState()
    {
        string state = amountKilled.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.amountKilled = System.Int32.Parse(state);
        UpdateState();
    }
}
