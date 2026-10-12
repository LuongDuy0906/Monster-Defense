using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartNewGame()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.allLevels[0]);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
