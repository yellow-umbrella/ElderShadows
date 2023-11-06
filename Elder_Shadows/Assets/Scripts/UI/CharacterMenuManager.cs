using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMenuManager : MonoBehaviour
{
    [SerializeField] private StatsPageUIManager statsPage;
    
    private void OnEnable()
    {
        statsPage.UpdateStatsPage(CharacterController.instance.characterData);
    }
}
