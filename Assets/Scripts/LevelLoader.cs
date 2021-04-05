using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene(Constants.MENU_SCENE);
    }
    
    public void LoadAbout()
    {
        SceneManager.LoadScene(Constants.ABOUT_SCENE);
    }
    
    public void LoadGameplay()
    {
        SceneManager.LoadScene(Constants.GAMEPLAY_SCENE);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}