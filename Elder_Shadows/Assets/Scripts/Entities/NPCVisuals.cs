using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NPCVisuals : MonoBehaviour
{
    [SerializeField] protected NPC npc;
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI replicText;
    [SerializeField] private float showingTime;

    protected virtual void Start()
    {
        npc.OnShowReplic += ShowReplic;
    }

    protected virtual void OnDestroy()
    {
        npc.OnShowReplic -= ShowReplic;
    }

    protected virtual void ShowReplic(string text, Action finishDialog)
    {
        replicText.text = text;
        ShowDialog();
        StartCoroutine(WaitAndHideDialog(finishDialog));
    }

    protected virtual void ShowDialog()
    {
        dialogPanel.SetActive(true);
    }

    private IEnumerator WaitAndHideDialog(Action finishDialog)
    {
        yield return new WaitForSeconds(showingTime);
        HideDialog();
        finishDialog();
    }

    protected virtual void HideDialog()
    {
        dialogPanel.SetActive(false);
    }
}
