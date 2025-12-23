using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class HighScoreEntry
{
    public string playerName;
    public float time;
    public DateTime date;
    public bool isCurrentPlayer;

    public HighScoreEntry(string name, float timeScore)
    {
        playerName = name;
        time = timeScore;
        date = DateTime.Now;
        isCurrentPlayer = false;
    }
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    private const string HIGH_SCORE_KEY = "ParkourHighScores";
    private const int MAX_STORED_SCORES = 15;
    private const int DISPLAY_SCORES = 10;

    private List<HighScoreEntry> highScores = new List<HighScoreEntry>();
    private string currentPlayerName = "Player";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ADD THIS METHOD (missing in your version):
    public bool CheckNewRecord(float currentTime)
    {
        float bestTime = GetBestTime();

        if (bestTime == float.MaxValue || currentTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", currentTime);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }

    // ADD THIS METHOD TOO:
    public float GetBestTime()
    {
        if (highScores.Count > 0)
            return highScores[0].time;

        // Fallback to PlayerPrefs
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        return bestTime;
    }

    public void SetCurrentPlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name))
            currentPlayerName = name;
    }

    public int AddScore(float time)
    {
        HighScoreEntry newEntry = new HighScoreEntry(currentPlayerName, time);
        newEntry.isCurrentPlayer = true;

        foreach (var entry in highScores)
            entry.isCurrentPlayer = false;

        highScores.Add(newEntry);
        highScores.Sort((a, b) => a.time.CompareTo(b.time));

        if (highScores.Count > MAX_STORED_SCORES)
            highScores.RemoveAt(MAX_STORED_SCORES);

        SaveHighScores();
        return highScores.IndexOf(newEntry) + 1;
    }

    public List<HighScoreEntry> GetDisplayScores()
    {
        List<HighScoreEntry> displayScores = new List<HighScoreEntry>();

        int count = Mathf.Min(DISPLAY_SCORES, highScores.Count);
        for (int i = 0; i < count; i++)
        {
            displayScores.Add(highScores[i]);
        }

        HighScoreEntry currentPlayerEntry = highScores.Find(e => e.isCurrentPlayer);
        if (currentPlayerEntry != null)
        {
            int currentPlayerIndex = highScores.IndexOf(currentPlayerEntry);

            if (currentPlayerIndex >= DISPLAY_SCORES)
            {
                if (displayScores.Count >= DISPLAY_SCORES)
                {
                    displayScores.Add(new HighScoreEntry("...", 0f));
                }

                displayScores.Add(currentPlayerEntry);
            }
        }

        return displayScores;
    }

    public int GetCurrentPlayerPosition()
    {
        HighScoreEntry currentPlayerEntry = highScores.Find(e => e.isCurrentPlayer);
        if (currentPlayerEntry == null) return 0;

        return highScores.IndexOf(currentPlayerEntry) + 1;
    }

    public int GetTotalScoresCount()
    {
        return highScores.Count;
    }

    public bool IsNewHighScore(float time)
    {
        if (highScores.Count < MAX_STORED_SCORES)
            return true;

        return time < highScores[MAX_STORED_SCORES - 1].time;
    }

    public float GetCurrentPlayerBestTime()
    {
        HighScoreEntry currentPlayerEntry = highScores.Find(e => e.isCurrentPlayer);
        if (currentPlayerEntry != null)
            return currentPlayerEntry.time;

        return float.MaxValue;
    }

    private void SaveHighScores()
    {
        string json = JsonUtility.ToJson(new HighScoreWrapper(highScores));
        PlayerPrefs.SetString(HIGH_SCORE_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadHighScores()
    {
        if (PlayerPrefs.HasKey(HIGH_SCORE_KEY))
        {
            string json = PlayerPrefs.GetString(HIGH_SCORE_KEY);
            HighScoreWrapper wrapper = JsonUtility.FromJson<HighScoreWrapper>(json);
            highScores = wrapper.highScores ?? new List<HighScoreEntry>();
        }
        else
        {
            highScores = new List<HighScoreEntry>();
        }
    }

    public void ClearHighScores()
    {
        highScores.Clear();
        PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class HighScoreWrapper
    {
        public List<HighScoreEntry> highScores;
        public HighScoreWrapper(List<HighScoreEntry> scores) { highScores = scores; }
    }
}