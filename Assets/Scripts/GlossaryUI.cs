using UnityEngine;
using TMPro; // Required for TextMeshProUGUI
using System.Text; // Required for StringBuilder

public class GlossaryUI : MonoBehaviour
{
    public static GlossaryUI Instance;

    public TextMeshProUGUI glossaryContentText; // Assign a TextMeshProUGUI component in the Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method when the glossary panel is opened to populate the content
    public void OpenPanel()
    {
        PopulateGlossaryContent();
        gameObject.SetActive(true); // Ensure the panel is active
        Debug.Log("[GlossaryUI] Glossary Panel Opened.");
    }

    private void PopulateGlossaryContent()
    {
        if (glossaryContentText == null)
        {
            Debug.LogError("[GlossaryUI] 'glossaryContentText' is not assigned in the Inspector.");
            return;
        }

        if (GlossaryManager.Instance == null)
        {
            Debug.LogError("[GlossaryUI] GlossaryManager.Instance not found. Cannot populate glossary content.");
            glossaryContentText.text = "Glossary data not available.";
            return;
        }

        if (GlossaryManager.Instance.glossaryEntries == null || GlossaryManager.Instance.glossaryEntries.Count == 0)
        {
            glossaryContentText.text = "No glossary entries found.";
            return;
        }

        StringBuilder contentBuilder = new StringBuilder();
        foreach (var entry in GlossaryManager.Instance.glossaryEntries)
        {
            contentBuilder.AppendLine($"<b>{entry.term}:</b> {entry.definition}\n");
        }
        glossaryContentText.text = contentBuilder.ToString();
    }
}