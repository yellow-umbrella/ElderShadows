using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIManager : MonoBehaviour
{
    [SerializeField] GameObject tradingWindow;
    [SerializeField] GameObject itemSlot;
    [SerializeField] Transform traderContent;
    private Trader trader;
    private Dictionary<ItemObject, GameObject> traderItems = new Dictionary<ItemObject, GameObject>();

    public void ShowTradingWindow(Trader trader)
    {
        tradingWindow.SetActive(true);
        this.trader = trader;
        UpdateTraderSection();
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

    private void UpdateTraderSlot(ItemObject item)
    {
        Trader.ItemInfo itemInfo = trader.currentItems[item];
        GameObject obj = traderItems[item];
        // only items bought from player have limited amount
        if (itemInfo.amount <= 0 && itemInfo.fromPlayer)
        {
            obj.transform.Find("Image").GetComponentInChildren<Image>().sprite = null;
            obj.transform.Find("Image").GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        else
        {
            obj.transform.Find("Image").GetComponentInChildren<Image>().sprite = item.uiDisplay;
            obj.transform.Find("Image").GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = itemInfo.price.ToString("n0");
        }
    }

    public void ExitTrading()
    {
        tradingWindow.SetActive(false);
        trader.IsUIClosed = true;
    }
}
