using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeLocationUIManager : MonoBehaviour
{
    public Button basicAttackButton;

    private void Start()
    {
        Debug.Log(MapTypeController.landscapeType);
        basicAttackButton.onClick.AddListener(CharacterController.instance.OnAttackButtonPressed);
    }
}
