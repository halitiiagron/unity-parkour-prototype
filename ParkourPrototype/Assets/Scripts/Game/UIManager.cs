using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text finalTimeText;
    [SerializeField] private Text bestTimeText;
    [SerializeField] private Text newRecordText;  // Changed from GameObject to Text

    private void Start()
    {
        // Hide win panel on start
        if (winPanel != null) winPanel.SetActive(false);
        if (newRecordText != null) newRecordText.gameObject.SetActive(false);
    }

    public void UpdateTimerDisplay(string timeString)
    {
        if (timerText != null)
            timerText.text = timeString;
    }

    public void ShowWinScreen(float finalTime, bool isNewRecord)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);

            // Update text elements
            if (finalTimeText != null)
                finalTimeText.text = "Time: " + GameTimer.FormatTime(finalTime);

            // Show/hide new record text
            if (newRecordText != null)
                newRecordText.gameObject.SetActive(isNewRecord);

            // Update best time display
            float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
            if (bestTime > 0f && bestTimeText != null)
            {
                bestTimeText.text = "Best: " + GameTimer.FormatTime(bestTime);
            }
            else if (bestTimeText != null)
            {
                bestTimeText.text = "Best: --:--.--";
            }
        }
    }
}