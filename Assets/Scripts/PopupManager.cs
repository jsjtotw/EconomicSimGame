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

    // Request structure to store message and callback
    private struct PopupRequest
    {
        public string message;
        public Action<bool> onResponse;

        public PopupRequest(string message, Action<bool> onResponse)
        {
            this.message = message;
            this.onResponse = onResponse;
        }
    }

    public void ShowConfirmation(string message, Action<bool> onResponse)
    {
        popupQueue.Enqueue(new PopupRequest(message, onResponse));
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
                Debug.LogError("[PopupManager] Popup prefab missing Popup script component.");
                yield break;
            }

            // Show popup and wait for user response
            bool? response = null;
            popup.Show(currentRequest.message, confirmed => response = confirmed);

            // Wait until user responds
            while (response == null)
                yield return null;

            Destroy(popupInstance);

            // Resume the game clock after popup closes
            if (TimeManager.Instance != null)
                TimeManager.Instance.ResumeClock();

            // Invoke callback
            currentRequest.onResponse?.Invoke(response.Value);
        }

        isShowingPopup = false;
    }

    public void ShowMessage(string title, string message, Action onClose)
{
    // For simplicity, you could display a confirmation popup with a single "OK" button
    ShowConfirmation($"{title}\n\n{message}", confirmed => {
        onClose?.Invoke();
    });
}

}