using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreationUIManager : BaseUIManager
{
    public TMP_InputField name_input;

    public void CreateCharacter_LoadNextScene()
    {
        CharacterData character = new CharacterData(name_input.text);
        DataBridge.instance.Character_data = character;
        base.LoadNextScene();
    }
}
