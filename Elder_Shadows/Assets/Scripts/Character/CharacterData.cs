using UnityEngine;

public class CharacterData
{
    public CharacterData(string name_)
    {
        name = name_;
    }
    [Header("General Properties")]
    public string name;

    private int experience = 0;
    public int Experience
    {
        get { return experience; }
        set { experience = value; }
    }

    private int level = 1;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    
    private int statpoints = 0; //Determines the amount of unspent STATS upgrade points
    public int StatPoints
    {
        get { return statpoints; }
        set { statpoints = value; }
    }
    
    [Header("Upgradeable Stats")]
    private int strength; //Responcible for Health, Regeneration, Physical Damage and Physical Resistance, or Armor
    public int Strength
    {
        get { return strength; }
        set { strength = value; }

    }
    
    private int inteligence; //Responcible for Mana, Regeneration, Magic Damage and Magic Resistance
    public int Intelligence
    {
        get { return inteligence; }
        set { inteligence = value; }

    }
    
    private int agility; //Responcible for Attack Speed, Movement speed, Evasion
    public int Agility
    {
        get { return agility; }
        set { agility = value; }

    }

    [Header("Dependent Parameters")]
    [Header("=====Strength=====")]
    private float max_health = 100;
    public float MaxHealth
    {
        get { return max_health; }
        set { max_health = value; }
    }

    private float hregen = 0.1f;
    public float HRegen
    {
        get { return hregen; }
        set { hregen = value; }
    }

    private int phys_dmg = 50;
    public int PhysDmg
    {
        get { return phys_dmg; }
        set { phys_dmg = value; }
    }

    private float phys_res = 0f;
    public float PhysRes
    {
        get { return phys_res; }
        set { phys_res = value; }
    }
    
    [Header("=====Inteligence=====")]
    private float max_mana = 50f;
    public float MaxMana
    {
        get { return max_mana; }
        set { max_mana = value; }
    }
    
    private float mregen = 0.1f;
    public float MRegen 
    {
        get { return mregen; }
        set { mregen = value; }
    }
    
    private float magic_dmg = 5f;
    public float MagicDmg
    {
        get { return magic_dmg; }
        set { magic_dmg = value; }
    }
    
    private float magic_res = 0f;
    public float MagicRes
    {
        get { return magic_res; }
        set { magic_res = value; }
    }
    
    [Header("=====Agility=====")]
    private float atk_spd = 1f;
    public float AtkSpd
    {
        get { return atk_spd; }
        set { atk_spd = value; }
    }
    
    private float movespeed = 5f;
    public float Movespeed
    {
        get { return movespeed; }
        set { movespeed = value; }
    }
    
    private float evasion = 0f;
    public float Evasion
    {
        get { return evasion; }
        set { evasion = value; }
    }

    [Header("Other Parameters")] 
    private float atk_range = 1f;
    public float AttackRange
    {
        get { return atk_range; }
        set { atk_range = value; }
    }
    
    [Header("Current value Parameters")]
    private float current_health = 100f;
    public float CurrentHealth
    {
        get { return current_health; }
        set { current_health = value; }
    }
    
    private float current_mana = 50f;
    public float CurrentMana
    {
        get { return current_mana; }
        set { current_mana = value; }
    }
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
        for (int i = 1; i < data.Level; i++)
        {
            prevExpNeeded += expDifference;
            expDifference = (int)(expDifference * expDiffMultiplier); 
            expDiffMultiplier = Mathf.Pow(expDiffMultiplier, xpMultiplierBalancer); 
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
        data.CurrentHealth -= damage;
    }
    
    public bool AddExperience(int xp)
    {
        data.Experience += xp;
        if (data.Experience >= (prevExpNeeded + expDifference))
        {
            data.Level++;
            Debug.Log("Reached level " + data.Level);
            expDifference = GetNewExpDifference();
            data.StatPoints = 1 + (data.Level / 5); //Equasion to increase stat points gained over time, the bigger the constant divider the less points will be gained
            return true;
        }
        return false;
    }

    public int GetLevel()
    {
        return data.Level;
    }

    public void UpgradeStat(string stat)
    {
        switch (stat)
        {
            case "Strength":
                data.Strength++;
                data.StatPoints--;
                break;
            case "Agility":
                data.Agility++;
                data.StatPoints--;
                break;
            case "Inteligence":
                data.Intelligence++;
                data.StatPoints--;
                break;
        }
        Debug.Log(data.StatPoints);
    }
}