using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour, IAttackable
{
    [Header("General")] 
    public static CharacterController instance;
    public Joystick joystick;
    public CharacterData characterData;
    public CharacterDataManager dataManager;
    [SerializeField] public CombatController combat;
    [SerializeField] private CharacterVisionManager vision;
    public List<Buff> buffs = new List<Buff>();
    
    public InventoryObject inventory;
    public CharacterEquipmentManager equipment;

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
    }

    private void Start()
    {
        dataManager = new CharacterDataManager(characterData);
        combat.characterData = characterData;
        combat.dataManager = dataManager;
        equipment.dataManager = dataManager;
    }

    void Update()
    {
        characterData.current_health += characterData.hregen * Time.deltaTime;
        transform.position += new Vector3(joystick.Horizontal, joystick.Vertical, transform.position.z) * characterData.movespeed * Time.deltaTime;
    }

    public IAttackable.State TakeDamage(float damage, IAttackable.DamageType type, GameObject attacker)
    {
        if (type == IAttackable.DamageType.Physical &&  Random.Range(0f, 1f) < dataManager.GetAttributeValue(Attributes.Evasion))
        {
            return IAttackable.State.Alive;
        }

        var resistance =
            dataManager.GetAttributeValue(type == IAttackable.DamageType.Physical
                ? Attributes.PhysDmg
                : Attributes.MagDmg);
        var puredmg = (damage - resistance) >= 0 ? damage - resistance : 0;
        
        dataManager.DealDamage(puredmg);
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

    public void AddDebuff(Buff debuff)
    {
        
    }

    private void Die()
    {
        HideCharacter();
        Time.timeScale = 0f;
        //Destroy(gameObject);
        
        Debug.Log("YOU DIED!!!");
    }

    private void HideCharacter()
    {
        
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

    public void OnSkillButtonPressed(int skillID)
    {
        combat.CastSkill(characterData.skillsID[0]);
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
