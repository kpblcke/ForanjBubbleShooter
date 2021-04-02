using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour
{

    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }

}