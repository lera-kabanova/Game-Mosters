using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SocialPlatforms;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject btnShowLeaderboard;  // ������ ��� ����������� �������
    public GameObject leaderboardImage;  // ���������� Image ��� ����������� �������
    public Transform leaderboardContent;  // ��������� ��� ��������� ������
    public GameObject leaderboardItemPrefab;  // ������ �������� ������

    // ������ ����������� � ���� ������
    private string connectionString = "Data Source=DataBase.db";

    void Start()
    {
    }

    // ������� ��� ����������� ������� ��������
    public void ShowLeaderboard()
    {
        // ������� ������� ������� � ���������
        leaderboardImage.gameObject.SetActive(true);

        // �������� ������ ������
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // �������� ��������������� ������ �� ���� ������
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            // ������ ��� ��������� ������ ������
            string checkTablesQuery = "SELECT name FROM sqlite_master WHERE type='table';";
            using (SqliteCommand command = new SqliteCommand(checkTablesQuery, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("Table found: " + reader.GetString(0));
                    }
                }
            }

            // ������ ��� ��������� ������ �� ������� Leaderboard
            string selectLeaderboardQuery = @"
        SELECT Username, EarnedMoney 
        FROM Leaderboard 
        ORDER BY EarnedMoney DESC";
            using (SqliteCommand command = new SqliteCommand(selectLeaderboardQuery, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string username = reader.GetString(0);
                        double earnedMoney = reader.GetDouble(1);
                        Debug.Log($"{username}: {earnedMoney}");
                    }
                }
            }
        }

    }
}
