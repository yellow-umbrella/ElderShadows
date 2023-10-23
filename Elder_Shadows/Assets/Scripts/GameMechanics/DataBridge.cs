using UnityEngine;
using MyBox;

public class DataBridge : MonoBehaviour
{
    public static DataBridge instance;
    public CharacterData Character_data{get {return character_data;} set { character_data = value; CharacterDataLoader.SaveCharacter(value);}}
    private CharacterData character_data;
    [SerializeField] private SceneReference gameplay_scene;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        if (CheckForSavedData())
        {
            MainMenuUIManager.instance.SetNextScene(gameplay_scene);
            Debug.Log(character_data.name);
        }
    }

    private bool CheckForSavedData()
    {
        character_data = CharacterDataLoader.LoadCharacter();
        if (character_data == null)
            return false;
        //TODO: Add check for saved terrain data
        return true;
    }
}
