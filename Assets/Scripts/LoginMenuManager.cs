using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Data;
using UnityEngine.UI;
using System.Text;

public class LoginMenuManager : MonoBehaviour
{
  
    [SerializeField]
    private GameObject SelectedMenu;

    [SerializeField]
    private GameObject LoginMenu;

    [SerializeField]
    private TMP_InputField loginInput;    // Ввод логина

    [SerializeField]
    private TMP_InputField passwordInput;    // Ввод пароля

    [SerializeField]
    private TMP_Text ErrorLogin; // Текст для сообщений об ошибках или успехе

    void Start()
    {
        // Настраиваем поля ввода для отображения паролей как звездочек
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
        
        
    }

    // Метод для входа пользователя
    public void LoginUser()
    {
        string username = loginInput.text.Trim();
        string password = passwordInput.text.Trim();

        // Проверка ввода
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowErrorText("Please fill in all fields.");
            return;
        }

        // Проверка данных в базе
        string conn = "URI=file:" + Application.dataPath + "/Database.db"; // Путь к базе данных
        using (SqliteConnection connection = new SqliteConnection(conn))
        {
            connection.Open();

            string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login AND Password = @Password";
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Login", username);
                command.Parameters.AddWithValue("@Password", password);

                try
                {
                    int userExists = Convert.ToInt32(command.ExecuteScalar());
                    if (userExists > 0)
                    {
                        // Успешный вход
                        ShowErrorText("Login successful!");
                        SceneManager.LoadScene(1); // Замените на нужную сцену
                        PlayerPrefs.SetString("CurrentUsername", username);

                    }
                    else
                    {
                        // Неверный логин или пароль
                        ShowErrorText("Invalid username or password.");
                    }
                }
                catch (Exception e)
                {
                    ShowErrorText("Login error: " + e.Message);
                }
            }
        }
    }

    void ShowErrorText(string message)
    {
        ErrorLogin.text = message;
    }

    public void ClickBack()
    {
        LoginMenu.SetActive(false);
        SelectedMenu.SetActive(true);
    }
}
