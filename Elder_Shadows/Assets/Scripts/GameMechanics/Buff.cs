using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public BuffData data;
    public float duration;
    public float timeleft;
    public bool isActivated = false;

    public Buff(BuffData buffData, float buffDuration)
    {
        data = buffData;
        duration = buffDuration;
        timeleft = duration;
    }

    public IEnumerator ActivateBuff(CharacterController character)
    {
        switch (data.type)
        {
            case BuffData.BuffType.HealthOverTime:
                float timer = 0;
                while (timer < duration)
                {
                    if (data.isPercentile)
                        character.TakeDamage(character.characterData.current_health * data.value, data.dmgType,
                            character.gameObject);
                    else character.TakeDamage(data.value, data.dmgType, character.gameObject);
                    timer += data.tickrate;
                    yield return new WaitForSeconds(data.tickrate);
                }
                break;
            case BuffData.BuffType.AttributeMod:
                if(data.isPercentile)
                    character.dataManager.ModifyAttribute(data.attribute, character.dataManager.GetAttributeValue(data.attribute) * data.value);
                else character.dataManager.ModifyAttribute(data.attribute, data.value);
                
                yield return new WaitForSeconds(duration);
                
                if(data.isPercentile)
                    character.dataManager.ModifyAttribute(data.attribute, -character.dataManager.GetAttributeValue(data.attribute) * data.value);
                else character.dataManager.ModifyAttribute(data.attribute, -data.value);
                
                break;
            case BuffData.BuffType.ManaMod:
                /*timer = 0;
                while (timer < duration)
                {
                    if (data.isPercentile)
                        character.TakeDamage(character.characterData.current_health * data.value, data.dmgType,
                            character.gameObject);
                    else character.TakeDamage(data.value, data.dmgType, character.gameObject);
                    timer += data.tickrate;
                    yield return new WaitForSeconds(data.tickrate);
                }*/
                break;
                
        }
    }
}
