using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIManager : MonoBehaviour
{
    [SerializeField] private GameObject tradingWindow;
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private Transform traderContent;
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private ItemDatabaseObject itemDatabase;
    [SerializeField] private TextMeshProUGUI playerMoney;
    [SerializeField] private TextMeshProUGUI traderName;
    private Trader trader;
    private Dictionary<ItemObject, GameObject> traderItems = new Dictionary<ItemObject, GameObject>();
    private Dictionary<GameObject, InventorySlot> inventoryItems = new Dictionary<GameObject, InventorySlot>();
    private ItemObject itemToBuy;
    private GameObject itemToSell;

    public void ShowTradingWindow(Trader trader)
    {
        ClearTradingWindow();
        this.trader = trader;
        traderName.text = trader.name;
        UpdateTradingWindow();
        tradingWindow.SetActive(true);
    }

    private void ClearTradingWindow()
    {
        foreach (var obj in traderItems.Values)
        {
            Destroy(obj);
        }
        foreach (var obj in inventoryItems.Keys)
        {
            Destroy(obj);
        }
        traderItems = new Dictionary<ItemObject, GameObject>();
        inventoryItems = new Dictionary<GameObject, InventorySlot>();
        itemToBuy = null;
        itemToSell = null;
    }

    private void UpdateTradingWindow()
    {
        UpdateTraderSection();
        UpdateInventorySection();
        playerMoney.text = CharacterController.instance.dataManager.GetMoney().ToString("n0");
    }

    private void UpdateTraderSection()
    {
        foreach (var tradeItem in trader.currentItems)
        {
            if (!traderItems.ContainsKey(tradeItem.Key))
            {
                GameObject obj = Instantiate(itemSlot, traderContent);
                traderItems.Add(tradeItem.Key, obj);
            }
            UpdateTraderSlot(tradeItem.Key);
        }
    }

    private void UpdateInventorySection()
    {
        InventorySlot[] inventory = CharacterController.instance.inventory.GetSlots;
        foreach (var slot in inventory)
        {
            if (!inventoryItems.ContainsValue(slot))
            {
                GameObject obj = Instantiate(itemSlot, inventoryContent);
                inventoryItems.Add(obj, slot);
            }
        }
        foreach (var obj in inventoryItems.Keys)
        {
            UpdateInventorySlot(obj);
        }
    }

    private void UpdateInventorySlot(GameObject slotObj)
    {
        TradingItemSlot itemSlot = slotObj.GetComponent<TradingItemSlot>();
        InventorySlot slot = inventoryItems[slotObj];
        itemSlot.Manager = this;
        if (slot.item.Id >= 0)
        {
            itemSlot.UpdateSlot(itemDatabase.ItemObjects[slot.item.Id], itemDatabase.ItemObjects[slot.item.Id].basePrice.ToString("n0"), slot.amount.ToString("n0"));
        } else
        {
            itemSlot.UpdateSlot(null, "", "");
        }
    }

    private void UpdateTraderSlot(ItemObject item)
    {
        Trader.ItemInfo itemInfo = trader.currentItems[item];
        TradingItemSlot itemSlot = traderItems[item].GetComponent<TradingItemSlot>();
        itemSlot.Manager = this;
        // only items bought from player have limited amount
        if (itemInfo.fromPlayer)
        {
            if (itemInfo.amount <= 0)
            {
                itemSlot.UpdateSlot(null, "", "");
            } else
            {
                itemSlot.UpdateSlot(item, itemInfo.price.ToString("n0"), itemInfo.amount.ToString("n0"));
            }
        }
        else
        {
            itemSlot.UpdateSlot(item, itemInfo.price.ToString("n0"), "");
        }
    }

    public void SelectItem (ItemObject item, GameObject obj)
    {
        if (obj.transform.parent == traderContent)
        {
            if (itemToBuy != null && traderItems[itemToBuy] != obj) 
            { 
                traderItems[itemToBuy].GetComponent<TradingItemSlot>().OnDeselect();
            }
            if (itemToSell != null)
            {
                itemToSell.GetComponent<TradingItemSlot>().OnDeselect();
            }
            itemToBuy = item;
        } else
        {
            if (itemToSell != null && itemToSell != obj)
            {
                itemToSell.GetComponent<TradingItemSlot>().OnDeselect();
            }
            if (itemToBuy != null)
            {
                traderItems[itemToBuy].GetComponent<TradingItemSlot>().OnDeselect();
            }
            itemToSell = obj;
        }
    }

    public void ExitTrading()
    {
        tradingWindow.SetActive(false);
        trader.IsUIClosed = true;
    }

    public void Buy()
    {
        if (itemToBuy == null) { return; }
        trader.Buy(itemToBuy, 1);
        UpdateTradingWindow();
    }

    public void Sell()
    {
        if (itemToSell == null || inventoryItems[itemToSell].item.Id < 0) { return; }
        trader.Sell(itemDatabase.ItemObjects[inventoryItems[itemToSell].item.Id], 1);
        UpdateTradingWindow();
    }
}
