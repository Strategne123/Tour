using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void LoadMultiplayer()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadSinglePlayer()
    {
        SceneManager.LoadScene(2);
    }

    public void Quit() => Application.Quit();
}
