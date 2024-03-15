using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType 
{
    Food,
    Helmet,
    Weapon,
    Shield,
    Boots,
    Chest,
    Default,
    Accessory,
    Quest,
    Material,
}

public enum Attributes
{
    MaxHealth,
    HRegen,
    PhysDmg,
    PhysRes,
    MaxMana,
    Mregen,
    MagDmg, 
    MagRes,
    MoveSpeed,
    Evasion,
    AtkRange
}
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]
public class ItemObject : ScriptableObject
{

    public Sprite uiDisplay;
    public bool stackable;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    public Item data = new Item();
    public int basePrice;

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }


}

[System.Serializable]
public class Item
{
    public string Name;
    public int Id = -1;
    public ItemBuff[] buffs;
    public Item()
    {
        Name = "";
        Id = -1;
    }
    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.data.Id;
        buffs = new ItemBuff[item.data.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max)
            {
                attribute = item.data.buffs[i].attribute
            };
        }
    }
}

[System.Serializable]
public class ItemBuff : IModifier
{
    public Attributes attribute;
    public float value;
    public float min;
    public float max;
    public ItemBuff(float _min, float _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }

    public void AddValue(ref float baseValue)
    {
        baseValue += value;
    }

    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}