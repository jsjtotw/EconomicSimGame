using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class TimeControlUI : MonoBehaviour
{
    public TimeManager timeManager;

    public Button speedCycleButton;
    public Button backButton;
    public TextMeshProUGUI speedCycleText;

    private int stateIndex = 0; // 0=normal, 1=double, 2=quadruple
    private bool isPaused = false;

    private float doubleTapTime = 0.3f; // max interval between taps to consider double tap
    private float lastTapTime = 0f;

    private void Awake()
    {
        speedCycleButton.onClick.RemoveAllListeners();
        speedCycleButton.onClick.AddListener(OnSpeedCycleClicked);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void Start()
    {
        SetTimeState(stateIndex); // Start at normal speed
    }

    private void OnSpeedCycleClicked()
    {
        float timeSinceLastTap = Time.time - lastTapTime;
        lastTapTime = Time.time;

        if (timeSinceLastTap <= doubleTapTime)
        {
            // Double tap detected → toggle pause/resume
            if (isPaused)
            {
                // Resume with previous speed
                isPaused = false;
                SetTimeState(stateIndex);
            }
            else
            {
                // Pause
                isPaused = true;
                timeManager.SetSpeed(0);
                speedCycleText.text = "||";
            }
        }
        else
        {
            // Single tap → only cycle if not paused
            if (!isPaused)
            {
                stateIndex = (stateIndex + 1) % 3; // cycle among 0,1,2
                SetTimeState(stateIndex);
            }
        }
    }

    private void SetTimeState(int index)
    {
        switch (index)
        {
            case 0:
                timeManager.SetSpeed(1);
                speedCycleText.text = ">";
                break;
            case 1:
                timeManager.SetSpeed(2);
                speedCycleText.text = ">>";
                break;
            case 2:
                timeManager.SetSpeed(4);
                speedCycleText.text = ">>>>";
                break;
        }
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
