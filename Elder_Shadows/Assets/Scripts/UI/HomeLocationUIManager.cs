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
        basicAttackButton.onClick.AddListener(CharacterController.instance.OnAttackButtonPressed);
    }
}
