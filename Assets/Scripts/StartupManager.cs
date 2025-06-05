// StartupManager.cs
// Manages the initial game flow, including company selection and scene loading.

using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    // Reference to the CompanySelectionPanel UI.
    public CompanySelectionPanel selectionPanel;

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        Debug.Log("[StartupManager] Initialized on MainMenu scene.");
    }

    /// <summary>
    /// Public method to initiate the game start process,
    /// which now involves opening the company selection panel.
    /// This method is intended to be called by a UI button.
    /// </summary>
    public void StartGame()
    {
        Debug.Log("[StartupManager] Opening company selection panel...");
        // Open the UI panel for company selection.
        selectionPanel.OpenPanel();
    }

    /// <summary>
    /// Internal method called by CompanySelectionPanel after a company is chosen.
    /// Loads the main game scene.
    /// </summary>
    public void StartGameInternal()
    {
        Debug.Log("[StartupManager] StartGameInternal() called. Loading GameScene...");
        // Load the main game scene. Ensure "GameScene" matches your actual scene name.
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("[StartupManager] ExitGame() called. Quitting application...");
        // Handle quitting differently in the editor vs. a build.
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
