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

    public GameObject scorePrefab; // Префаб для отображения одного результата
    public Transform scoreParent; // Родительский объект для списка рекордов

    public GameObject HighScoresBoard; // Панель для отображения таблицы рекордов

    public int topRanks; // Количество отображаемых лучших результатов

    void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/DataBase.db";
        ShowScores(); // Отображение рекордов при запуске
    }

    // Метод для записи нового результата в таблицу
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
                dbCmd.Parameters.Add(new SqliteParameter("@Date", DateTime.Now)); // Добавляем текущую дату и время
                dbCmd.ExecuteNonQuery();
            }
        }
        ShowScores(); // Обновляем таблицу рекордов сразу после записи
    }

    // Метод для извлечения данных из базы
    private void GetScores()
    {
        highScores.Clear(); // Очищаем список перед загрузкой данных

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
        highScores.Sort(); // Сортируем результаты, если это необходимо
    }

    // Метод для удаления результата по ID
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
        ShowScores(); // Обновляем таблицу после удаления
    }

    // Метод для отображения таблицы рекордов
    private void ShowScores()
    {
        GetScores(); // Загружаем актуальные данные из базы

        // Удаляем старые элементы перед добавлением новых
        foreach (Transform child in scoreParent)
        {
            Destroy(child.gameObject);
        }

        // Создаем записи для лучших результатов
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

    // Показать панель таблицы рекордов
    public void ShowHighScore()
    {
        HighScoresBoard.SetActive(true);
        ShowScores(); // Обновить отображение при открытии панели
    }

    // Скрыть панель таблицы рекордов
    public void Back()
    {
        HighScoresBoard.SetActive(false);
    }
}

