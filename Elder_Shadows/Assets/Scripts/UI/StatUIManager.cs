using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private Button info;
    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private Button upgrade;
    [SerializeField] private string statKey;

    public void UpdateData(int newValue)
    {
        value.text = newValue.ToString();
    }
    
    public void SetUpgrade(bool value)
    {
        upgrade.gameObject.SetActive(value);
    }

    public void AddListenerToUpgradeButton(Func<string, int> UpgradeStat)
    {
        upgrade.onClick.AddListener(() => UpgradeStat(statKey));
    }
}
