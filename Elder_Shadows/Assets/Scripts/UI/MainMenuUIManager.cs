using MyBox;
using UnityEngine;

public class MainMenuUIManager : BaseUIManager
{
    public static MainMenuUIManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SetNextScene(SceneReference next_scene)
    {
        base.nextScene = next_scene;
    }
}
