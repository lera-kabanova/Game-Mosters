using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class HighScoreManager : MonoBehaviour
{
    private string connectionString;

    private List<HighScore> highScores = new List<HighScore>();

    public GameObject scorePrefab; // ������ ��� ����������� ������ ����������
    public Transform scoreParent; // ������������ ������ ��� ������ ��������

    public GameObject HighScoresBoard; // ������ ��� ����������� ������� ��������

    public int topRanks; // ���������� ������������ ������ �����������

    void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/DataBase.db";
        ShowScores(); // ����������� �������� ��� �������
    }

    // ����� ��� ������ ������ ���������� � �������
    private void InsertScore(string name, int newScore)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO HighScores(Name, Score, Date) VALUES(@Name, @Score, @Date)";
                dbCmd.CommandText = sqlQuery;

                dbCmd.Parameters.Add(new SqliteParameter("@Name", name));
                dbCmd.Parameters.Add(new SqliteParameter("@Score", newScore));
                dbCmd.Parameters.Add(new SqliteParameter("@Date", DateTime.Now)); // ��������� ������� ���� � �����
                dbCmd.ExecuteNonQuery();
            }
        }
        ShowScores(); // ��������� ������� �������� ����� ����� ������
    }

    // ����� ��� ���������� ������ �� ����
    private void GetScores()
    {
        highScores.Clear(); // ������� ������ ����� ��������� ������

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT PlayerID, Name, Score, Date FROM HighScores ORDER BY Score DESC";
                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        highScores.Add(new HighScore(
                            reader.GetInt32(0), // PlayerID
                            reader.GetInt32(2), // Score
                            reader.GetString(1), // Name
                            reader.GetDateTime(3) // Date
                        ));
                    }
                }
            }
        }
        highScores.Sort(); // ��������� ����������, ���� ��� ����������
    }

    // ����� ��� �������� ���������� �� ID
    private void DeleteScore(int id)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "DELETE FROM HighScores WHERE PlayerID = @ID";
                dbCmd.CommandText = sqlQuery;
                dbCmd.Parameters.Add(new SqliteParameter("@ID", id));
                dbCmd.ExecuteNonQuery();
            }
        }
        ShowScores(); // ��������� ������� ����� ��������
    }

    // ����� ��� ����������� ������� ��������
    private void ShowScores()
    {
        GetScores(); // ��������� ���������� ������ �� ����

        // ������� ������ �������� ����� ����������� �����
        foreach (Transform child in scoreParent)
        {
            Destroy(child.gameObject);
        }

        // ������� ������ ��� ������ �����������
        for (int i = 0; i < topRanks; i++)
        {
            if (i < highScores.Count)
            {
                GameObject tmpObject = Instantiate(scorePrefab, scoreParent);
                HighScore tmpScore = highScores[i];

                tmpObject.GetComponent<HighScoreScript>().SetScore(
                    tmpScore.Name,
                    tmpScore.Score.ToString(),
                    "#" + (i + 1).ToString()
                );

                tmpObject.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }
    }

    // �������� ������ ������� ��������
    public void ShowHighScore()
    {
        HighScoresBoard.SetActive(true);
        ShowScores(); // �������� ����������� ��� �������� ������
    }

    // ������ ������ ������� ��������
    public void Back()
    {
        HighScoresBoard.SetActive(false);
    }
}

