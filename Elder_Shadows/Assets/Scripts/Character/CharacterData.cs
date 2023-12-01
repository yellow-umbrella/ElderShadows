using UnityEngine;

public class CharacterData
{
    public CharacterData(string name_)
    {
        name = name_;
        experience = 0;
        level = 1;
        statpoints = 0;
        strength = 1;
        inteligence = 1;
        agility = 1;
        max_health = 0;
        hregen = 0.1f;
        phys_dmg = 0;
        max_mana = 0;
        mregen = 0.1f;
        magic_dmg = 0;
        atk_spd = 1;
        movespeed = 5f;
        atk_range = 1f;
        current_health = max_health;
        current_mana = max_mana;
    }
    
    [Header("General Properties")]
    public string name;
    public int experience = 0;
    public int level = 1;
    public int statpoints = 0; //Determines the amount of unspent STATS upgrade points

    [Header("Upgradeable Stats")]
    public int strength; //Responcible for Health, Regeneration, Physical Damage and Physical Resistance, or Armor
    public int inteligence; //Responcible for Mana, Regeneration, Magic Damage and Magic Resistance
    public int agility; //Responcible for Attack Speed, Movement speed, Evasion

    [Header("Dependent Parameters")]
    [Header("=====Strength=====")]
    public float max_health;
    public float hregen;
    public int phys_dmg = 50;
    public float phys_res = 0f;

    [Header("=====Inteligence=====")]
    public float max_mana = 50f;
    public float mregen = 0.1f;
    public float magic_dmg = 5f;
    public float magic_res = 0f;
    
    [Header("=====Agility=====")]
    public float atk_spd = 1f;
    public float movespeed = 5f;
    public float evasion = 0f; // cant go higher than 0.8, Clamped at CharacterDataManager.SetupAttributes()
    
    [Header("Other Parameters")] 
    public float atk_range = 1f;

    [Header("Current value Parameters")]
    public float current_health = 100f;
    public float current_mana = 50f;
}

public class CharacterDataManager
{
    private CharacterData data;
    private int prevExpNeeded = 0;
    private int expDifference = 50;
    private float expDiffMultiplier = 1.5f; //needed to increase xp needed to advance to next level
    private float xpMultiplierBalancer = 0.95f; //needed to balance multiplier so it wont get impossible to gain xp in later levels or opposite
    
    public CharacterDataManager(CharacterData data_)
    {
        data = data_;
        for (int i = 1; i < data.level; i++)
        {
            prevExpNeeded += expDifference;
            expDifference = (int)(expDifference * expDiffMultiplier); 
            expDiffMultiplier = Mathf.Pow(expDiffMultiplier, xpMultiplierBalancer);
            SetupAttributes();
        }
    }

    //Sets up all the attributes depending on main stats 
    private void SetupAttributes()
    {
        data.max_health = data.strength * 10;
        data.phys_dmg = data.strength;
        data.hregen = data.strength * 0.05f;
        data.phys_res = data.strength;
        
        data.max_mana = data.inteligence * 10;
        data.magic_dmg = data.inteligence;
        data.mregen = data.inteligence * 0.025f;
        data.magic_res = data.inteligence;

        data.movespeed = 3f + data.agility * 0.025f;
        data.evasion = Mathf.Clamp(data.agility * 0.005f, 0, 0.8f);
        data.atk_spd = Mathf.Clamp(25 / data.agility, 1.25f, 0.25f);
    }

    //set value to negative to reduce attribute
    public void ModifyAttribute(Attributes attribute, float value)
    {
        switch (attribute)
        {
            case Attributes.MaxHealth:
                data.max_health += value;
                break;
            case Attributes.HRegen:
                data.hregen += value;
                break;
            case Attributes.PhysDmg:
                data.phys_dmg += (int)value;
                break;
            case Attributes.PhysRes:
                data.phys_res += value;
                break;
            case Attributes.MaxMana:
                data.max_mana += value;
                break;
            case Attributes.Mregen:
                data.mregen += value;
                break;
            case Attributes.MagDmg:
                data.magic_dmg += value;
                break;
            case Attributes.MagRes:
                data.magic_res += value;
                break;
            case Attributes.MoveSpeed:
                data.movespeed += value;
                break;
            case Attributes.Evasion:
                data.evasion += value;
                break;
            case Attributes.AtkRange:
                data.atk_range += value;
                break;
        }
    }

    private int GetNewExpDifference()
    {
        int newExpDiff = (int)(expDifference * expDiffMultiplier);
        expDiffMultiplier = Mathf.Pow(expDiffMultiplier, xpMultiplierBalancer);
        return newExpDiff;
    }

    public void DealDamage(float damage)
    {
        data.current_health -= damage;
    }
    
    public bool AddExperience(int xp)
    {
        data.experience += xp;
        if (data.experience >= (prevExpNeeded + expDifference))
        {
            data.level++;
            Debug.Log("Reached level " + data.level);
            expDifference = GetNewExpDifference();
            data.statpoints = 1 + (data.level / 5); //Equasion to increase stat points gained over time, the bigger the constant divider the less points will be gained
            return true;
        }
        return false;
    }

    public int GetLevel()
    {
        return data.level;
    }

    public void UpgradeStat(string stat)
    {
        switch (stat)
        {
            case "Strength":
                data.strength++;
                data.statpoints--;
                break;
            case "Agility":
                data.agility++;
                data.statpoints--;
                break;
            case "Inteligence":
                data.inteligence++;
                data.statpoints--;
                break;
        }
    }
}