using UnityEngine;
using System.Collections.Generic;

public class GlossaryManager : MonoBehaviour
{
    public static GlossaryManager Instance;

    public List<GlossaryEntry> glossaryEntries { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGlossaryFromJson();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadGlossaryFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("glossary_data"); // Assumes JSON is named "glossary_data.json"
        if (jsonFile != null)
        {
            GlossaryListWrapper wrapper = JsonUtility.FromJson<GlossaryListWrapper>(jsonFile.text);
            glossaryEntries = wrapper.entries;
            Debug.Log($"[GlossaryManager] Loaded {glossaryEntries.Count} glossary entries from JSON.");
        }
        else
        {
            Debug.LogError("[GlossaryManager] 'glossary_data.json' not found in Resources folder. Glossary will not be loaded.");
            glossaryEntries = new List<GlossaryEntry>(); // Initialize as empty list to avoid NullReferenceException
        }
    }
}