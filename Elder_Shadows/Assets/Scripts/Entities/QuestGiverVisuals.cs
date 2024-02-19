using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class QuestGiverVisuals : NPCVisuals
{
    private QuestGiver questGiver;
    private SpriteRenderer sprite;

    private void Awake()
    {
        questGiver = npc as QuestGiver;
        sprite = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        questGiver.onActiveQuestStateChange += ChangeAppearance;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        questGiver.onActiveQuestStateChange -= ChangeAppearance;
    }

    private void ChangeAppearance(Quest.QuestState state)
    {
        sprite.color = Color.white;
        switch (state)
        {
            case Quest.QuestState.CAN_START:
                sprite.color = Color.red;
                break;
            case Quest.QuestState.IN_PROGRESS:
                sprite.color = Color.gray;
                break;
            case Quest.QuestState.CAN_FINISH:
                sprite.color = Color.green;
                break;
        }
    }
}
