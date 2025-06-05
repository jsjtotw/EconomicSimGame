using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("Popup Prefab")]
    public GameObject popupPrefab;  // Assign your popup prefab here (must contain Popup component)

    // Request structure to store message, title, and callbacks for both types of popups
    private struct PopupRequest
    {
        public string title;            // Added title for both types of popups
        public string message;
        public Action<bool> onConfirmationResponse; // For confirmation popups
        public Action onMessageClose;              // For simple message popups

        // Constructor for confirmation popups
        public PopupRequest(string title, string message, Action<bool> onConfirmationResponse)
        {
            this.title = title;
            this.message = message;
            this.onConfirmationResponse = onConfirmationResponse;
            this.onMessageClose = null; // Ensure this is null for confirmation requests
        }

        // Constructor for message-only popups
        public PopupRequest(string title, string message, Action onMessageClose)
        {
            this.title = title;
            this.message = message;
            this.onMessageClose = onMessageClose; // FIX: Corrected parameter name here
            this.onConfirmationResponse = null; // Ensure this is null for message requests
        }
    }

    private Queue<PopupRequest> popupQueue = new Queue<PopupRequest>();
    private bool isShowingPopup = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (popupPrefab == null)
                Debug.LogError("[PopupManager] Popup prefab is not assigned!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Public method to request a confirmation popup (Yes/No buttons).
    /// </summary>
    /// <param name="title">The title of the popup.</param>
    /// <param name="message">The main message content.</param>
    /// <param name="onResponse">Callback invoked with true for confirm, false for cancel.</param>
    public void ShowConfirmation(string title, string message, Action<bool> onResponse)
    {
        popupQueue.Enqueue(new PopupRequest(title, message, onResponse));
        if (!isShowingPopup)
            StartCoroutine(ProcessQueue());
    }

    /// <summary>
    /// Public method to request a simple message popup (OK button).
    /// </summary>
    /// <param name="title">The title of the popup.</param>
    /// <param name="message">The main message content.</param>
    /// <param name="onClose">Callback invoked when the OK button is clicked.</param>
    public void ShowMessage(string title, string message, Action onClose = null)
    {
        popupQueue.Enqueue(new PopupRequest(title, message, onClose));
        if (!isShowingPopup)
            StartCoroutine(ProcessQueue());
    }


    private IEnumerator ProcessQueue()
    {
        isShowingPopup = true;

        while (popupQueue.Count > 0)
        {
            PopupRequest currentRequest = popupQueue.Dequeue();

            // Pause the game clock when popup appears
            if (TimeManager.Instance != null)
                TimeManager.Instance.PauseClock();

            // Instantiate popup prefab
            GameObject popupInstance = Instantiate(popupPrefab, transform);
            Popup popup = popupInstance.GetComponent<Popup>();

            if (popup == null)
            {
                Debug.LogError("[PopupManager] Popup prefab missing Popup script component. Make sure your popup prefab has a 'Popup' script attached!");
                Destroy(popupInstance); // Clean up the instantiated object.
                yield break; // Exit if critical component is missing.
            }

            // Show popup based on request type
            bool? response = null;
            if (currentRequest.onConfirmationResponse != null)
            {
                // It's a confirmation request
                popup.ShowConfirmation(currentRequest.title, currentRequest.message, confirmed => response = confirmed);
            }
            else // It's a message-only request
            {
                popup.ShowMessage(currentRequest.title, currentRequest.message, () => response = true); // Simulate 'true' for message dismissal
            }

            // Wait until user responds (response will be set by the Popup script's callback)
            while (response == null)
                yield return null;

            Destroy(popupInstance); // Destroy the popup instance after it's dismissed.

            // Resume the game clock after popup closes
            if (TimeManager.Instance != null)
                TimeManager.Instance.ResumeClock();

            // Invoke original callback based on request type
            if (currentRequest.onConfirmationResponse != null)
            {
                currentRequest.onConfirmationResponse?.Invoke(response.Value);
            }
            else // It's a message-only request
            {
                currentRequest.onMessageClose?.Invoke();
            }
        }

        isShowingPopup = false;
    }
}