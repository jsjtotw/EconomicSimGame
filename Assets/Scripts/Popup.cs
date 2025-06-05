// Popup.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;
    public TextMeshProUGUI titleText; // Added for title

    private Action<bool> onResponse;
    private Action onMessageClose; // For simple message popups

    // Call this to setup and show a confirmation popup
    public void ShowConfirmation(string title, string message, Action<bool> callback)
    {
        // FIX: Ensure titleText is not null before setting its text.
        // If titleText is not assigned in the Inspector, it will be null.
        if (titleText != null)
        {
            titleText.text = title; // Set the title
        }
        else
        {
            Debug.LogWarning("[Popup] titleText is not assigned in the Inspector for confirmation popup. Title will not be displayed.");
        }
        
        messageText.text = message;
        onResponse = callback;
        onMessageClose = null; // Ensure this is null for confirmation

        confirmButton.gameObject.SetActive(true); // Ensure confirm button is active
        cancelButton.gameObject.SetActive(true);  // Ensure cancel button is active

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        gameObject.SetActive(true);
    }

    // Call this to setup and show a simple message popup (OK button)
    public void ShowMessage(string title, string message, Action callback = null)
    {
        // FIX: Ensure titleText is not null before setting its text.
        if (titleText != null)
        {
            titleText.text = title; // Set the title
        }
        else
        {
            Debug.LogWarning("[Popup] titleText is not assigned in the Inspector for simple message popup. Title will not be displayed.");
        }
        
        messageText.text = message;
        onMessageClose = callback;
        onResponse = null; // Ensure this is null for message popup

        confirmButton.gameObject.SetActive(true);  // OK button
        cancelButton.gameObject.SetActive(false); // Hide cancel button

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnMessageConfirmClicked);

        gameObject.SetActive(true);
    }


    private void OnConfirmClicked()
    {
        onResponse?.Invoke(true);
        gameObject.SetActive(false); // Hide the popup
    }

    private void OnCancelClicked()
    {
        onResponse?.Invoke(false);
        gameObject.SetActive(false); // Hide the popup
    }

    private void OnMessageConfirmClicked()
    {
        onMessageClose?.Invoke();
        gameObject.SetActive(false); // Hide the popup
    }
}