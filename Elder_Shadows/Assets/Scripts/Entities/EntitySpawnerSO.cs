using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntitySpawnerSO", menuName = "Entities/EntitySpawnerSO")]
public class EntitySpawnerSO : ScriptableObject
{
    public List<BaseEntity> nightEntities = new List<BaseEntity>();
    public List<BaseEntity> dayEntities = new List<BaseEntity>();
}
