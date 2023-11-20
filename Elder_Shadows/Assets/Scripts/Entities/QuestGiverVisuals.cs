using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class QuestGiverVisuals : MonoBehaviour
{
    [SerializeField] private QuestGiver questGiver;

    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        questGiver.onActiveQuestStateChange += ChangeAppearance;
    }
    private void OnDestroy()
    {
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
