// File: Assets/Scripts/GameFlow/WinLoseManager.cs
using UnityEngine;
using TMPro; // For UI text
using UnityEngine.SceneManagement; // For restarting scene

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance { get; private set; }

    public GameObject winScreenPanel; // Assign in Inspector
    public GameObject loseScreenPanel; // Assign in Inspector
    public TextMeshProUGUI winLoseMessageText; // For a dynamic message

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        winScreenPanel?.SetActive(false);
        loseScreenPanel?.SetActive(false);
    }

    public void ShowWinScreen()
    {
        winScreenPanel?.SetActive(true);
        if (winLoseMessageText != null)
        {
            winLoseMessageText.text = "Congratulations! You've reached your financial goals and become a true tycoon!";
        }
        // Optionally display stats, achievements etc.
    }

    public void ShowLoseScreen()
    {
        loseScreenPanel?.SetActive(true);
        if (winLoseMessageText != null)
        {
            winLoseMessageText.text = "Game Over! Your company faced insurmountable challenges and went bankrupt.";
        }
        // Optionally display reasons for loss, final stats etc.
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Resume time
        // Reload the current game scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1; // Resume time
        // Load your main menu scene
        SceneManager.LoadScene("MainMenu"); // Make sure "MainMenu" scene exists and is in Build Settings
    }
}