using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //Will change our scene to the string passed in
    public void ChangeScene(string Main)
    {
        SceneManager.LoadScene(Main);
    }

    //Reloads the current scene we are in
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Loads our Title scene. Must be called Title exactly
    public void ToTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    //Gets our active scenes name
    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    //Quits our game
    public void QuitGame()
    {
        Application.Quit();
    }
}
