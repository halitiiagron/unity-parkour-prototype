using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // DEBUG: Log EVERYTHING that enters
        Debug.Log("=== FINISH ZONE TRIGGER ===");
        Debug.Log("Object: " + other.name);
        Debug.Log("Tag: " + other.tag);
        Debug.Log("Layer: " + other.gameObject.layer);
        Debug.Log("Position: " + other.transform.position);

        if (other.CompareTag("Player"))
        {
            Debug.Log("✓ PLAYER detected in finish zone!");
            CompleteLevel();
        }
        else
        {
            Debug.Log("✗ Not player - wrong tag");
        }
    }

    private void CompleteLevel()
    {
        Debug.Log("=== COMPLETING LEVEL ===");

        // Try to get timer
        GameTimer timer = FindObjectOfType<GameTimer>();
        if (timer == null)
        {
            Debug.LogError("❌ GameTimer not found!");
            return;
        }

        timer.StopTimer();
        float finalTime = timer.GetCurrentTime();
        Debug.Log("Final time: " + GameTimer.FormatTime(finalTime));

        // Try to show win screen
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager not found!");
            return;
        }

        // Check high score
        HighScoreManager highScore = FindObjectOfType<HighScoreManager>();
        bool isNewRecord = false;

        if (highScore != null)
        {
            isNewRecord = highScore.CheckNewRecord(finalTime);
        }
        else
        {
            Debug.LogWarning("HighScoreManager not found, checking manually");
            float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
            if (finalTime < bestTime)
            {
                PlayerPrefs.SetFloat("BestTime", finalTime);
                isNewRecord = true;
            }
        }

        Debug.Log("New Record? " + isNewRecord);
        uiManager.ShowWinScreen(finalTime, isNewRecord);
        Debug.Log("✓ Win screen should be visible");
    }
}