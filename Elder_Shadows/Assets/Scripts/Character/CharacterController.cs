using System;
using UnityEngine;

public class CharacterController : MonoBehaviour, IAttackable
{
    public Joystick joystick;
    [SerializeField] private float movespeed;
    public CharacterData characterData;

    private void Awake()
    {
        characterData = DataBridge.instance.Character_data;
    }

    void Update()
    {
        transform.position += new Vector3(joystick.Horizontal, joystick.Vertical, transform.position.z) * movespeed * Time.deltaTime;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        characterData.health -= damage;
        if (characterData.health <= 0)
        {
            Die();
        }
        else
        {
            OnHit(attacker);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("YOU DIED!!!");
    }

    private void OnHit(GameObject attacker)
    {
        Debug.Log("You are taking damage, better run or fight back!");
    }
}
