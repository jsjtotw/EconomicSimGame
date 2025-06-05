using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    private Action<bool> onResponse;

    // Call this to setup and show the popup
    public void Show(string message, Action<bool> callback)
    {
        messageText.text = message;
        onResponse = callback;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        gameObject.SetActive(true);
    }

    private void OnConfirmClicked()
    {
        onResponse?.Invoke(true);
    }

    private void OnCancelClicked()
    {
        onResponse?.Invoke(false);
    }
}
