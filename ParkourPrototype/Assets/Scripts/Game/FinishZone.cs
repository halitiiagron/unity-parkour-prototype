using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        Debug.Log("=== FINISH ZONE TRIGGERED ===");

        // Get timer
        GameTimer timer = GameTimer.Instance;
        if (timer == null)
        {
            Debug.LogError("GameTimer not found!");
            return;
        }

        // Stop timer and get final time
        timer.StopTimer();
        float finalTime = timer.GetCurrentTime();
        Debug.Log($"Final time: {GameTimer.FormatTime(finalTime)}");

        // Check for new record using HighScoreManager
        bool isNewRecord = false;
        HighScoreManager highScoreManager = HighScoreManager.Instance;

        if (highScoreManager != null)
        {
            isNewRecord = highScoreManager.CheckNewRecord(finalTime);
            Debug.Log($"New record? {isNewRecord}");

            // Also add to high score list
            int position = highScoreManager.AddScore(finalTime);
            Debug.Log($"Position on leaderboard: #{position}");
        }
        else
        {
            Debug.LogWarning("HighScoreManager not found, using PlayerPrefs fallback");
            float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
            if (finalTime < bestTime)
            {
                PlayerPrefs.SetFloat("BestTime", finalTime);
                PlayerPrefs.Save();
                isNewRecord = true;
            }
        }

        // Show win screen
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowWinScreen(finalTime, isNewRecord);
        }
        else
        {
            Debug.LogError("UIManager not found!");
        }

        // Show high score panel (TAB panel)
        HighScoreUI highScoreUI = FindObjectOfType<HighScoreUI>();
        if (highScoreUI != null)
        {
            highScoreUI.ShowResultsScreen(finalTime);
        }
        else
        {
            Debug.LogWarning("HighScoreUI not found - TAB leaderboard won't show");
        }

        Debug.Log("=== LEVEL COMPLETE ===");
    }
}