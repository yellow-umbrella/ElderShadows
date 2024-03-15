using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TraderItems", menuName = "Entities/TraderItemsSO")]
public class TraderItemsSO : ScriptableObject
{
    public float priceMult = 1.2f;
    public List<TradeItem> tradeItems;

    [Serializable]
    public struct TradeItem
    {
        public ItemObject item;
        public int levelRequirement;
        public int trustRequirement;
    }
}
