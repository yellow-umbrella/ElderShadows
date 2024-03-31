using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public ItemObject item;
    [SerializeField]private static float lifetime = 180f;
    private float age = 0f;
    public UnityEvent onDespawn;
    public UnityEvent onPickedUp;

    private void Update()
    {
        if (age >= lifetime)
        {
            Despawn();
        }
        else age += Time.deltaTime;
    }

    private void Despawn()
    {
        onDespawn.Invoke();
        Destroy(gameObject);
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        GetComponentInChildren<SpriteRenderer>().sprite = item.uiDisplay;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
    }
}
