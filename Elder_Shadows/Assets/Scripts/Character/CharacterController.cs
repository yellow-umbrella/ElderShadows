using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Joystick joystick;
    [SerializeField] private float movespeed;

    void Update()
    {
        transform.position += new Vector3(joystick.Horizontal, joystick.Vertical, transform.position.z) * movespeed * Time.deltaTime;
    }
}
