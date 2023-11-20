using System;
using System.Collections.Generic;

using UnityEngine;

public class CharacterController : MonoBehaviour, IAttackable
{
    [Header("General")] 
    public static CharacterController instance;
    public Joystick joystick;
    public CharacterData characterData;
    private CharacterDataManager dataManager;
    [SerializeField] private CombatController combat;
    //public InventoryObject inventory;

    [Header("Vision")]
    private float timeBetweenAggroChecks = 1;
    private float timeToNextAggroCheck;
    [SerializeField] private Collider2D seeingRange;
    private List<Collider2D> intruders = new List<Collider2D>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this.gameObject);
               
#if UNITY_EDITOR
        characterData = new CharacterData("TEST");
#else
        characterData = DataBridge.instance.Character_data;
#endif
        dataManager = new CharacterDataManager(characterData);
        combat.characterData = characterData;
        combat.dataManager = dataManager;
    }

    void Update()
    {
        transform.position += new Vector3(joystick.Horizontal, joystick.Vertical, transform.position.z) * characterData.movespeed * Time.deltaTime;
    }

    public IAttackable.State TakeDamage(int damage, GameObject attacker)
    {
        dataManager.DealDamage(damage);
        if (characterData.current_health <= 0)
        {
            Die();
            return IAttackable.State.Dead;
        }
        else
        {
            OnHit(attacker);
        }
        return IAttackable.State.Alive;
    }

    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("YOU DIED!!!");
    }

    private void OnHit(GameObject attacker)
    {
        Debug.Log("You are taking damage, better run or fight back!");
    }
    
    protected void CheckForIntruders(LayerMask intrudersMask)
    {
        timeToNextAggroCheck -= Time.deltaTime;
        if (timeToNextAggroCheck <= 0)
        {
            timeToNextAggroCheck = timeBetweenAggroChecks;
        } else
        {
            return;
        }

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(intrudersMask);
        contactFilter.useTriggers = true;

        seeingRange.OverlapCollider(contactFilter, intruders);

        foreach (Collider2D collider in intruders)
        {
            GameObject potentialTarget = collider.gameObject;
            if (!potentialTarget.Equals(gameObject))
            {
                if (seeingRange.OverlapPoint(potentialTarget.transform.position))
                {
                    OnSee(potentialTarget);
                }
            }
        }
    }

    public void OnAttackButtonPressed()
    {
        combat.TryAttack();
    }

    private void OnSee(GameObject target)
    {
        //TODO: add UI to show targetted entity HP or name
    }

    public void UpgradeStat(string stat)
    {
        Debug.Log("Upgraded " + stat);
        dataManager.UpgradeStat(stat);
    }
}
