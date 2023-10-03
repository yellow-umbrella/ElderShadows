using UnityEngine;
using MyBox;
using UnityEngine.SceneManagement;

public class BaseUIManager : MonoBehaviour
{
    [SerializeField]private SceneReference nextScene;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene.SceneName);
    }

    public void LoadScene(SceneReference scene)
    {
        SceneManager.LoadScene(scene.SceneName);
    }
}
