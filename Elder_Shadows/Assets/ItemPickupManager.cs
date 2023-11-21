using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager : MonoBehaviour
{
    [SerializeField] private CharacterController character;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item")
        {
            Item _item = new Item(other.GetComponent<GroundItem>().item);
            Debug.Log(_item.Id);
            character.inventory.AddItem(_item, 1);
            Destroy(other.gameObject);
        }
    }
}
