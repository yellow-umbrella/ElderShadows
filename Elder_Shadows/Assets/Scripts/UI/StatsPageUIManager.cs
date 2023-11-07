using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsPageUIManager : MonoBehaviour
{
    [SerializeField] private StatUIManager strengthManager;
    [SerializeField] private StatUIManager agilityManager;
    [SerializeField] private StatUIManager inteligenceManager;
    [SerializeField] private TextMeshProUGUI statpoints;

    private void Start()
    {
        strengthManager.AddListenerToUpgradeButton(UpgradeStat);
        agilityManager.AddListenerToUpgradeButton(UpgradeStat);
        inteligenceManager.AddListenerToUpgradeButton(UpgradeStat);
    }

    public void UpdateStatsPage(CharacterData data)
    {
        int statpoints_ = data.statpoints;
        statpoints.gameObject.SetActive(Convert.ToBoolean(statpoints_));
        statpoints.text = "Available points: " + statpoints_;

        strengthManager.UpdateData(data.strength);
        agilityManager.UpdateData(data.agility);
        inteligenceManager.UpdateData(data.inteligence);
        
        SetUpgrade(Convert.ToBoolean(statpoints_));
    }

    private void SetUpgrade(bool value)
    {
        strengthManager.SetUpgrade(value);
        agilityManager.SetUpgrade(value);
        inteligenceManager.SetUpgrade(value);
    }

    public int UpgradeStat(string stat)
    {
        CharacterController.instance.UpgradeStat(stat);
        UpdateStatsPage(CharacterController.instance.characterData);

        return 0;
    }
}
