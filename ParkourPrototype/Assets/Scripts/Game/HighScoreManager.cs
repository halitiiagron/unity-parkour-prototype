using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    private const string BEST_TIME_KEY = "BestTime";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public bool CheckNewRecord(float currentTime)
    {
        float bestTime = PlayerPrefs.GetFloat(BEST_TIME_KEY, float.MaxValue);

        // If no previous record or current time is better
        if (bestTime == float.MaxValue || currentTime < bestTime)
        {
            PlayerPrefs.SetFloat(BEST_TIME_KEY, currentTime);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }

    public float GetBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat(BEST_TIME_KEY, 0f);
        return (bestTime == 0f) ? float.MaxValue : bestTime;
    }

    public void ResetBestTime()
    {
        PlayerPrefs.DeleteKey(BEST_TIME_KEY);
        PlayerPrefs.Save();
    }
}