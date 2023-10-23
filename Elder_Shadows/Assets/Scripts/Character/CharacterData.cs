using UnityEngine;

public class CharacterData
{
    public CharacterData(string name_)
    {
        name = name_;
        health = 100;
        level = 1;
        experience = 0;
        phys_dmg_multiplier = 1;
    }

    public string name;
    public int health;
    public int level;
    public int experience;
    public int phys_dmg_multiplier;
}