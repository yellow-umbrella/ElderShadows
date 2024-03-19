using UnityEngine;

public class CharacterData
{
    public CharacterData(string name_)
    {
        name = name_;
        experience = 0;
        level = 1;
        statpoints = 0;
        money = 10;
        trust = 0;
        strength = 1;
        inteligence = 1;
        agility = 1;
        max_health = 10;
        hregen = 0.1f;
        phys_dmg = 0;
        max_mana = 10;
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
    public int money;
    public int trust;

    [Header("Upgradeable Stats")]
    public int strength; //Responcible for Health, Regeneration, Physical Damage and Physical Resistance, or Armor
    public int inteligence; //Responcible for Mana, Regeneration, Magic Damage and Magic Resistance
    public int agility; //Responcible for Attack Speed, Movement speed, Evasion

    [Header("Dependent Parameters")]
    [Header("=====Strength=====")]
    public float max_health;
    public float hregen;
    public float phys_dmg = 50;
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

    public int[] skillsID;
    
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
        InitializeData();
    }

    public void InitializeData()
    {
        for (int i = 1; i <= data.level; i++)
        {
            prevExpNeeded += expDifference;
            expDifference = (int)(expDifference * expDiffMultiplier); 
            expDiffMultiplier = Mathf.Pow(expDiffMultiplier, xpMultiplierBalancer);
            SetupAttributes();
            
            Debug.Log("Character Health: " + data.current_health + "/" + data.max_health);
            CharacterBarsUIManager.instance.UpdateExpBar(expDifference, data.experience);
            CharacterBarsUIManager.instance.UpdateHealthBar(data.max_health, data.current_health);
            CharacterBarsUIManager.instance.UpdateManaBar(data.max_mana, data.current_mana);
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
        data.atk_spd = Mathf.Clamp(25 / data.agility, 0.25f, 1.25f);
    }

    //set value to negative to reduce attribute
    public void ModifyAttribute(Attributes attribute, float value)
    {
        switch (attribute)
        {
            case Attributes.MaxHealth:
                data.max_health += value;
                data.current_health += value;
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
                data.current_mana += value;
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
        
        CharacterBarsUIManager.instance.UpdateHealthBar(data.max_health, data.current_health);
        CharacterBarsUIManager.instance.UpdateManaBar(data.max_mana, data.current_mana);
    }

    public float GetAttributeValue(Attributes attribute)
    {
        float value = 0;
        
        switch (attribute)
        {
            case Attributes.MaxHealth:
                value = data.max_health;
                break;
            case Attributes.HRegen:
                value = data.hregen;
                break;
            case Attributes.PhysDmg:
                value = data.phys_dmg;
                break;
            case Attributes.PhysRes:
                value = data.phys_res;
                break;
            case Attributes.MaxMana:
                value = data.max_mana;
                break;
            case Attributes.Mregen:
                value = data.mregen;
                break;
            case Attributes.MagDmg:
                value = data.magic_dmg;
                break;
            case Attributes.MagRes:
                value = data.magic_res;
                break;
            case Attributes.MoveSpeed:
                value = data.movespeed;
                break;
            case Attributes.Evasion:
                value = data.evasion;
                break;
            case Attributes.AtkRange:
                value = data.atk_range;
                break;
        }

        return value;
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
        CharacterBarsUIManager.instance.UpdateHealthBar(data.max_health, data.current_health);
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
        CharacterBarsUIManager.instance.UpdateExpBar(expDifference, data.experience);
        return false;
    }

    public int GetLevel()
    {
        return data.level;
    }

    public int GetMoney() { return data.money; }
    public int GetTrust() { return data.trust; }

    public bool AddMoney(int amount)
    {
        if (data.money + amount < 0)
        {
            return false;
        }
        data.money += amount;
        return true;
    }

    public void AddTrust(int amount)
    {
        data.trust = Mathf.Max(0, data.trust + amount);
    }

    public void UpgradeStat(string stat)
    {
        switch (stat)
        {
            case "Strength":
                data.strength++;
                data.statpoints--;
                
                data.max_health += 10;
                data.phys_dmg++;
                data.hregen += 0.05f;
                data.phys_res++;
                break;
            case "Agility":
                data.agility++;
                data.statpoints--;
                
                data.movespeed += 0.025f;
                data.evasion = Mathf.Clamp(data.evasion + 0.005f, 0, 0.8f);
                data.atk_spd = Mathf.Clamp(data.atk_spd + 25 / 1, 0.25f, 1.25f);
                break;
            case "Inteligence":
                data.inteligence++;
                data.statpoints--;
                
                data.max_mana += 10;
                data.magic_dmg++;
                data.mregen += 0.025f;
                data.magic_res++;
                break;
        }
    }
}