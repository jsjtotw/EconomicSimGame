using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("[StartupManager] Initialized on MainMenu scene.");
    }

    public void StartGame()
    {
        Debug.Log("[StartupManager] StartGame() called. Loading GameScene...");
        SceneManager.LoadScene("GameScene"); // Replace with your actual game scene name
    }

    public void ExitGame()
    {
        Debug.Log("[StartupManager] ExitGame() called. Quitting application...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
