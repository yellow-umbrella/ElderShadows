using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class BaseEntityVisuals : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI entityName;
    [SerializeField] private bool isMiniboss;
    
    private BaseEntity entity;
    private MovementController movementController;
    private EntityBT entityBT;
    private Animator animator;
    private AudioSource audioSource;
    private EntityAttackSO attack;
    private EntityAttackVFX attackVFX;

    private const string WALK = "Walk";
    private const string IDLE = "Idle";
    private const string TARGETED_ATTACK = "TargetedAttack";
    private const string DIRECTION = "Direction";

    private float maxValue;

    private void Start()
    {
        movementController = GetComponentInParent<MovementController>();
        entity = GetComponentInParent<BaseEntity>();
        entityBT = GetComponentInParent<EntityBT>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        maxValue = entity.Health;
        healthBar.fillAmount = 1;
        entityName.text = entity.Info.displayName;
        SetNameColor();
        entity.OnStartAttack += Entity_OnStartAttack;
    }

    private void Entity_OnStartAttack(EntityAttackSO attack)
    {
        // trigger right animation based on attack type
        if (animator.runtimeAnimatorController != null)
        {
            animator.SetInteger(DIRECTION, entity.AttackDirection);
            animator.SetTrigger(TARGETED_ATTACK);
        }
        this.attack = attack;
        attackVFX = null;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        entity.IsAttacking = true;
        if (attack.attackSound != null)
        {
            audioSource.clip = attack.attackSound;
            audioSource.Play();
        }
        if (attack.attackVFX != null)
        {
            Instantiate(attack.attackVFX, entity.transform).TryGetComponent(out attackVFX);
        }
        attackVFX?.SetupVFX(attack);
        //yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        // wait for the middle of attack animation
        if (animator.runtimeAnimatorController != null)
        {
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= .5f);
        } else
        {
            yield return new WaitForSeconds(1f);
        }

        entity.CanInflictDamage = true;
        attackVFX?.OnInflictDamage();
        
        // wait for the end of attack animation
        if (animator.runtimeAnimatorController != null)
        {
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        entity.IsAttacking = false;
        attackVFX?.OnEndAttack();
    }

    private void Update()
    {
        healthBar.fillAmount = entity.Health / maxValue;
        SetHealthbarColor();
        if (animator.runtimeAnimatorController == null) { return; }
        if (entityBT.ActiveNode is WalkNode)
        {
            animator.SetInteger(DIRECTION, movementController.MovementDirection);
            animator.SetBool(WALK, true);
        } else
        {
            animator.SetBool(WALK, false);
        }
        animator.SetBool(IDLE, !(entityBT.ActiveNode is WalkNode) 
                            && !(entityBT.ActiveNode is AttackTargetNode));
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
