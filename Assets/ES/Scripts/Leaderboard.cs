using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.IO;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class Leaderboard : MonoBehaviour
{
    [Header("Leaderboard UI")]
    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> scores;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject namesObject;
    [SerializeField] private GameObject scoresObject;

    [Header("Player")]
    [SerializeField] private int playerScore;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private GameObject submitButton;

    [Header("Input")]
    [SerializeField] private TMP_InputField inputField;

    private LeaderboardData leaderboardData = new LeaderboardData();

    private string SavePath => Path.Combine(Application.persistentDataPath,"leaderboard.json");

    private void Start()
    {

    }

    void OnEnable()
    {
        LoadLeaderboard();
        RefreshUI();

        playerScoreText.text = playerScore.ToString();
        namesObject.SetActive(false);
        scoresObject.SetActive(false);
    }

    void OnDisable()
    {
        namesObject.SetActive(false);
        scoresObject.SetActive(false);
    }

    public void GoHome()
    {
        //menuUI.SetActive(true);
        gameObject.SetActive(false);
        menuUI.SetActive(true);
        mainMenuUI.SetActive(true);
    }

    public void SubmitUserInput()
    {
        if (!inputField.gameObject.activeSelf) return;

        namesObject.SetActive(true);
        scoresObject.SetActive(true);

        string playerName = inputField.text;

        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        LeaderboardEntry newEntry =
            new LeaderboardEntry
            {
                playerName = playerName,
                score = playerScore
            };

        int insertIndex = leaderboardData.entries.Count;

        for (int i = 0; i < leaderboardData.entries.Count; i++)
        {
            if (playerScore >
                leaderboardData.entries[i].score)
            {
                insertIndex = i;
                break;
            }
        }

        leaderboardData.entries.Insert(insertIndex,newEntry);

        if (leaderboardData.entries.Count > names.Count)
        {
            leaderboardData.entries.RemoveAt(
                leaderboardData.entries.Count - 1);
        }

        SaveLeaderboard();
        RefreshUI();

        inputField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        playerScoreText.gameObject.SetActive(false);

        
    }

    private void RefreshUI()
    {
        for (int i = 0; i < names.Count; i++)
        {
            if (i < leaderboardData.entries.Count)
            {
                names[i].gameObject.SetActive(true);
                scores[i].gameObject.SetActive(true);

                names[i].text = leaderboardData.entries[i].playerName;

                scores[i].text = leaderboardData.entries[i].score.ToString();
            }
            else
            {
                names[i].gameObject.SetActive(false);
                scores[i].gameObject.SetActive(false);
            }
        }
    }

    private void SaveLeaderboard()
    {
        string json =
            JsonUtility.ToJson(
                leaderboardData,
                true);

        File.WriteAllText(SavePath, json);

        Debug.Log(
            "Leaderboard saved to: " + SavePath);
    }

    private void LoadLeaderboard()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);

            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
        }
    }


    [ContextMenu("Clear Leaderboard Save")]
    private void ClearLeaderboardSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);

            leaderboardData = new LeaderboardData();

            RefreshUI();

            Debug.Log("Leaderboard save deleted");
        }
    }
}