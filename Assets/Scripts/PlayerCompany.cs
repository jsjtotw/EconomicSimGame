using UnityEngine;

public class PlayerCompany : MonoBehaviour
{
    public static PlayerCompany Instance;

    public CompanyType SelectedType { get; private set; }
    public bool IsCompanyChosen { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCompany(CompanyType type)
    {
        SelectedType = type;
        IsCompanyChosen = true;
        Debug.Log($"[PlayerCompany] Company selected: {type}");
    }
}
