using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CharacterMenuManager : MonoBehaviour
{
    [SerializeField] private StatsPageUIManager statsPage;

    private void Start()
    {
        StartCoroutine(HideMenu());
    }

    private IEnumerator HideMenu()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        try
        {
            statsPage.UpdateStatsPage(CharacterController.instance.characterData);
        }
        catch
        {
            
        }
    }
}
