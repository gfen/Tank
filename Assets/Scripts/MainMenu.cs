using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnOnePlayerButtonClick()
    {
        GameMode.SetSingle(true, "MainMenu");
        SceneManager.LoadScene("Lobby");
    }

    public void OnTwoPlayerButtonClick()
    {
        GameMode.SetSingle(false, "MainMenu");
        SceneManager.LoadScene("Lobby");
    }

    public void OnMapEditorButtonClick()
    {
        SceneManager.LoadScene("MapEditor");
    }
}
