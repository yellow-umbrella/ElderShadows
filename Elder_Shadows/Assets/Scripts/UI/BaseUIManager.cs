using UnityEngine;
using MyBox;
using UnityEngine.SceneManagement;

public class BaseUIManager : MonoBehaviour
{
    [SerializeField]protected SceneReference nextScene;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene.SceneName);
    }

    public void LoadScene(SceneReference scene)
    {
        SceneManager.LoadScene(scene.SceneName);
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
