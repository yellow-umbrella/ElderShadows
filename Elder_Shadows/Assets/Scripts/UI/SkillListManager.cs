using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillListManager : MonoBehaviour
{
    [SerializeField] private GameObject Skills;
    [SerializeField] private TextMeshProUGUI Icon;

    public void OnClick()
    {
        Skills.SetActive(!Skills.activeSelf);
        Icon.text = Skills.activeSelf ? "v" : ">";
    }
}
