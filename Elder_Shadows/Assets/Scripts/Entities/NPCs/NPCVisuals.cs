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
    [SerializeField] private Transform canvas;
    private float canvasOffset = .5f;

    protected virtual void Start()
    {
        npc.OnShowReplic += ShowReplic;
        npc.OnHideReplic += HideDialog;
    }

    protected virtual void OnDestroy()
    {
        npc.OnShowReplic -= ShowReplic;
        npc.OnHideReplic -= HideDialog;
    }

    protected virtual void ShowReplic(string text)
    {
        replicText.text = text;
        ShowDialog();
    }

    protected virtual void ShowDialog()
    {
        dialogPanel.SetActive(true);
        canvas.position += new Vector3(0, canvasOffset);
    }

    protected virtual void HideDialog()
    {
        dialogPanel.SetActive(false);
        canvas.position += new Vector3(0, -canvasOffset);
    }
}
