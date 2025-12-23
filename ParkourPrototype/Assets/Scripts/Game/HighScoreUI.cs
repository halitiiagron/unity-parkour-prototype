using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HighScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private Transform scoresContainer;
    [SerializeField] private GameObject scoreEntryPrefab;
    [SerializeField] private GameObject currentPlayerEntryPrefab;
    [SerializeField] private Text worldPositionText;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] private bool showDuringGame = true;

    private bool isVisible = false;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HideHighScores);

        HideHighScores();
    }

    void Update()
    {
        if (showDuringGame && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleHighScores();
        }
    }

    public void ToggleHighScores()
    {
        if (isVisible)
            HideHighScores();
        else
            ShowHighScores();
    }

    public void ShowHighScores()
    {
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(true);
            isVisible = true;
            PopulateHighScores();
        }
    }

    public void HideHighScores()
    {
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
            isVisible = false;
        }
    }

    // ADD/UPDATE THIS METHOD - Called from FinishZone
    public void ShowResultsScreen(float finalTime)
    {
        // Get position from HighScoreManager
        int position = 1;
        int totalPlayers = 1;

        HighScoreManager highScoreManager = HighScoreManager.Instance;
        if (highScoreManager != null)
        {
            position = highScoreManager.GetCurrentPlayerPosition();
            totalPlayers = highScoreManager.GetTotalScoresCount();
        }

        // Update world position text
        if (worldPositionText != null)
        {
            worldPositionText.text = $"#{position} / {totalPlayers} Worldwide";
        }

        // Show the panel
        ShowHighScores();
    }

    private void PopulateHighScores()
    {
        foreach (Transform child in scoresContainer)
        {
            Destroy(child.gameObject);
        }

        // Try to get real scores from HighScoreManager
        HighScoreManager highScoreManager = HighScoreManager.Instance;
        if (highScoreManager != null)
        {
            List<HighScoreEntry> scores = highScoreManager.GetDisplayScores();

            if (scores.Count == 0)
            {
                AddTestScore("No scores yet!", 0f, 0, false);
                return;
            }

            for (int i = 0; i < scores.Count; i++)
            {
                bool isCurrentPlayer = scores[i].isCurrentPlayer;
                string playerName = scores[i].playerName;

                // Handle "..." separator
                if (playerName == "..." && scores[i].time == 0f)
                {
                    AddTestScore("...", 0f, 0, false);
                    continue;
                }

                AddTestScore(playerName, scores[i].time, i + 1, isCurrentPlayer);
            }
        }
        else
        {
            // Fallback to test data
            AddTestScore("Player1", 45.67f, 1, false);
            AddTestScore("Player2", 52.34f, 2, false);
            AddTestScore("YOU", 55.89f, 3, true);
            AddTestScore("Player4", 61.23f, 4, false);
        }
    }

    private void AddTestScore(string name, float time, int rank, bool isCurrentPlayer)
    {
        GameObject prefab = isCurrentPlayer && currentPlayerEntryPrefab != null
            ? currentPlayerEntryPrefab
            : scoreEntryPrefab;

        if (prefab == null) return;

        GameObject entryObj = Instantiate(prefab, scoresContainer);
        Text textComp = entryObj.GetComponent<Text>();

        if (textComp != null)
        {
            string timeString = FormatTime(time);
            string rankString = GetRankString(rank);

            if (rank == 0) // For "..." separator
                textComp.text = "...";
            else
                textComp.text = $"{rankString}. {name} - {timeString}";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    private string GetRankString(int rank)
    {
        if (rank == 1) return "1st";
        if (rank == 2) return "2nd";
        if (rank == 3) return "3rd";
        if (rank > 3) return $"{rank}th";
        return "";
    }

    public void SetPlayerName(string name)
    {
        Debug.Log("Player name set to: " + name);
    }
}