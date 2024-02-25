using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseEntityVisuals : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI entityName;
    [SerializeField] private bool isMiniboss;
    private BaseEntity entity;
    private MovementController movementController;

    private float maxValue;

    private void Start()
    {
        movementController = GetComponentInParent<MovementController>();
        entity = GetComponentInParent<BaseEntity>();
        maxValue = entity.Health;
        healthBar.fillAmount = 1;
        entityName.text = entity.Info.displayName;
        SetNameColor();
        entity.OnStartAttack += Entity_OnStartAttack;
    }

    private void Entity_OnStartAttack(EntityAttackSO.AttackType attackType)
    {
        // trigger right animation based on attack type
        entity.CanInflictDamage = true;
        // set in the end of animation
        entity.IsAttacking = false;
    }

    private void Update()
    {
        healthBar.fillAmount = entity.Health / maxValue;
        SetHealthbarColor();
    }

    private void SetNameColor()
    {
        if (isMiniboss)
        {
            entityName.color = Color.red;
            entityName.fontStyle = FontStyles.Bold;
        } else if (entity.IsModified)
        {
            entityName.color = new Color(1f, 0.544f, 0.513f);
        } else
        {
            entityName.color = Color.white;
        }
    }

    private void SetHealthbarColor()
    {
        switch (entity.CurrentReaction)
        {
            case BaseEntity.Behavior.Aggressive:
                healthBar.color = Color.red;
                break;
            case BaseEntity.Behavior.Defensive:
                healthBar.color = new Color(1f, .49f, 0);
                break;
            case BaseEntity.Behavior.Relaxed:
                healthBar.color = new Color(1f, .745f, 0);
                break;
            case BaseEntity.Behavior.Scared:
                healthBar.color = new Color(0.57f, 0.71f, 0);
                break;
        }
    }
}
