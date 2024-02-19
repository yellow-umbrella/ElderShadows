using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Lines", menuName = "Entities/DialogLinesSO")]
public class DialogLinesSO : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] busyLines;

    [TextArea(2, 5)]
    public string[] gratitudeLines;
}
