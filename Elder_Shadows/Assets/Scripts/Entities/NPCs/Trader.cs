using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour
{
    public struct ItemInfo
    {
        public int price;
        public int amount;
        public bool fromPlayer;
    }

    public bool IsUIClosed { get; set; }
    public Dictionary<ItemObject, ItemInfo> currentItems { get; set; } = new Dictionary<ItemObject, ItemInfo>();

    [SerializeField] private TraderItemsSO itemsToSell;
    [SerializeField] private TradingUIManager ui;

    public void StartTrading()
    {
        SelectItems();
        IsUIClosed = false;
        ui.ShowTradingWindow(this);
    }

    private void SelectItems()
    {
        foreach (var tradeItem in itemsToSell.tradeItems)
        {
            bool req = CharacterController.instance.dataManager.GetLevel() >= tradeItem.levelRequirement
                        && CharacterController.instance.dataManager.GetTrust() >= tradeItem.trustRequirement;
            if (req && !currentItems.ContainsKey(tradeItem.item))
            {
                currentItems.Add(tradeItem.item, new ItemInfo
                {
                    price = (int)(tradeItem.item.basePrice * itemsToSell.priceMult),
                    amount = 0,
                    fromPlayer = false
                });
            }
        }
    }

    /// <summary>
    /// To buy item from trader
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns>True if transaction was successful</returns>
    public bool Buy(ItemObject item, int amount)
    {
        if (!currentItems.ContainsKey(item)) { return false; }
        ItemInfo info = currentItems[item];
        if (info.fromPlayer)
        {
            amount = Mathf.Min(amount, info.amount);
        }
        int price = amount * info.price;
        if (price > CharacterController.instance.dataManager.GetMoney()) { return false; }
        CharacterController.instance.dataManager.AddMoney(-price);
        CharacterController.instance.inventory.AddItem(new Item(item), amount);
        info.amount = Mathf.Max(0, info.amount - amount);
        currentItems[item] = info;
        return true;
    }

    /// <summary>
    /// To sell item to trader
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool Sell(ItemObject item, int amount)
    {
        if (!CharacterController.instance.inventory.RemoveItem(new Item(item), amount)) { return false; }
        int price = item.basePrice * amount;
        CharacterController.instance.dataManager.AddMoney(price);
        if (currentItems.ContainsKey(item))
        {
            ItemInfo info = currentItems[item];
            info.amount += amount;
            currentItems[item] = info;
        } else
        {
            currentItems.Add(item, new ItemInfo
            {
                price = (int)(item.basePrice * itemsToSell.priceMult),
                amount = amount,
                fromPlayer = true
            });
        }
        return true;
    }
}
