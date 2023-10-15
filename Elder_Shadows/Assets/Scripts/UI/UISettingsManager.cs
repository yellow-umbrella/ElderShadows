using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject startSettingsPanel;

    private GameObject activeSettingsPanel;

    public void ShowSettings(GameObject settingsPanel)
    {
        activeSettingsPanel?.SetActive(false);
        activeSettingsPanel = settingsPanel;
        activeSettingsPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        ShowSettings(startSettingsPanel);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }
}
