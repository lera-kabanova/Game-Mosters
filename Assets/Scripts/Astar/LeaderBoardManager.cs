using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SocialPlatforms;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject btnShowLeaderboard;  // Кнопка для отображения таблицы
    public GameObject leaderboardImage;  // Используем Image для отображения таблицы
    public Transform leaderboardContent;  // Контейнер для элементов списка
    public GameObject leaderboardItemPrefab;  // Префаб элемента списка

    // Строка подключения к базе данных
    private string connectionString = "Data Source=DataBase.db";

    void Start()
    {
    }

    // Функция для отображения таблицы рекордов
    public void ShowLeaderboard()
    {
        // Сделать видимой таблицу с рекордами
        leaderboardImage.gameObject.SetActive(true);

        // Очистить старые данные
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Получить отсортированные данные из базы данных
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            // Запрос для получения списка таблиц
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

            // Запрос для получения данных из таблицы Leaderboard
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
