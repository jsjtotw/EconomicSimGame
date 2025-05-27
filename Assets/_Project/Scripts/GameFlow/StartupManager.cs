// File: Assets/Scripts/GameFlow/StartupManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading
using UnityEngine.UI; // Required for Button components
using TMPro; // Required for TextMeshPro buttons/text

public class StartupManager : MonoBehaviour
{
    [Header("Main Menu UI References")]
    public Button startGameButton;
    public Button optionsButton; // Optional
    public Button exitButton;

    [Header("Scene Names")]
    public string gameSceneName = "GameScene"; // Make sure this matches your actual GameScene name

    void Awake()
    {
        // Ensure time is running for main menu animations/UI
        Time.timeScale = 1f; 
    }

    void Start()
    {
        // Assign button click listeners
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("StartGameButton not assigned in StartupManager!", this);
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(ShowOptions);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    public void StartGame()
    {
        Debug.Log("Loading Game Scene...");
        // This will load the GameScene, where GameManager and other systems will then initialize.
        // The CompanySelector in GameScene will automatically activate upon loading.
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowOptions()
    {
        // Implement your options panel display logic here
        Debug.Log("Showing Options (Not implemented yet)");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing in editor
#else
        Application.Quit(); // Quit the standalone application
#endif
    }
}