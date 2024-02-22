using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [HideInInspector]
    public int skillID;

    private void Start()
    {
        Destroy(this.gameObject);
    }
}
