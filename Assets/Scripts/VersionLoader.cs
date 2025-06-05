using UnityEngine;
using TMPro; // TextMeshPro namespace

public class VersionLoader : MonoBehaviour
{
    public TextMeshProUGUI versionText; // Assign in inspector

    void Start()
    {
        LoadVersion();
    }

    void LoadVersion()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("version");
        if (jsonFile != null)
        {
            VersionInfo info = JsonUtility.FromJson<VersionInfo>(jsonFile.text);
            versionText.text = $"{info.version} • {info.build} • © {info.studio}";
            Debug.Log("[VersionLoader] Loaded Version: " + versionText.text);
        }
        else
        {
            Debug.LogError("[VersionLoader] version.json not found in Resources.");
        }
    }
}
