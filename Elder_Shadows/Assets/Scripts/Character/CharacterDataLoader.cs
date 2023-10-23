using System.IO;
using UnityEngine;

public class CharacterDataLoader
{
    [SerializeField] private static string path = "/";

    public static CharacterData LoadCharacter()
    {
        if(File.Exists(Application.persistentDataPath + path + "CharacterData.json"))
        {
            string jsondata = File.ReadAllText(Application.persistentDataPath + path + "CharacterData.json");
            CharacterData character_data = JsonUtility.FromJson<CharacterData>(jsondata);
            return character_data;
        }
        else return null;
    }

    public static void SaveCharacter(CharacterData character_data)
    {
        string jsondata = JsonUtility.ToJson(character_data);
        File.WriteAllText(Application.persistentDataPath + path + "CharacterData.json", jsondata);
    }
}
