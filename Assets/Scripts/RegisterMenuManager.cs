using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using TMPro;
using System.Security.Cryptography;
using System.Text;

public class RegisterMenuManager : MonoBehaviour
{
    public SqliteConnection dbConnection;

    [SerializeField]
    private GameObject SelectedMenu;

    [SerializeField]
    private GameObject RegistrationMenu;


    [SerializeField]
    private TMP_InputField loginInput;    // ���� ������

    [SerializeField]
    private TMP_InputField passwordInput;    // ���� ������

    [SerializeField]
    private TMP_InputField confirmPasswordInput; // ���� ������������� ������

    [SerializeField]
    private TMP_Text registrationErrorText;  // ����� ��� ������

    [SerializeField]
    private Button ContinueButton; // ������ ��� �������� ������

    [SerializeField]
    private TMP_Text ErrorText; // ����� ��� ��������� �� ������� ��� ������


    void Start()
    {
        CreateUsersTable();
        CreateTablesIfNotExists();
        // ����������� ���� ����� ��� ����������� ������� ��� ���������
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;

        passwordInput.ForceLabelUpdate();
        confirmPasswordInput.ForceLabelUpdate();

    }
    void CreateUsersTable()
    {
        string conn = "URI=file:" + Application.dataPath + "/Database.db"; // ���� � ���� ������
        using (SqliteConnection connection = new SqliteConnection(conn))
        {
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL
                );";

            using (SqliteCommand command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

   public void CreateTablesIfNotExists()
    {

        string conn = "URI=file:" + Application.dataPath + "/Database.db";
        using (SqliteConnection connection = new SqliteConnection(conn))
        {
            connection.Open();

            string createUserStatsTableQuery = @"
            CREATE TABLE IF NOT EXISTS UserStatistics (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT NOT NULL,
            KilledMonsters INTEGER NOT NULL DEFAULT 0,
            EarnedMoney INTEGER NOT NULL DEFAULT 0
        );";

          
            using (SqliteCommand command = new SqliteCommand(createUserStatsTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            string createLeaderBoardTableQuery = @"
            CREATE TABLE IF NOT EXISTS HighScores (
            PlayerID INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE,
            Name TEXT,
            Score INTEGER,
            Date DATETIME DEFAULT CURRENT_DATE
            );";

            using (SqliteCommand command = new SqliteCommand(createLeaderBoardTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    
    }
 

    // ����� ��� �������� ���� �����������
    public void OpenRegistrationImage()
    {
        ErrorText.text = ""; // �������� ����� ���������
        loginInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
    }

    // ����� ��� ����������� ������������
   public void RegisterUser()
    {
        string username = loginInput.text.Trim();
        string password = passwordInput.text.Trim();
        string confirmPassword = confirmPasswordInput.text.Trim();


        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowErrorText("All fields must be filled out.");
            return;
        }

        // �������� ������
        if (!ValidateInputs(username, password, confirmPassword, out string errorMessage))
        {
            ShowErrorText(errorMessage); 
            return;
        }

        // ���������� ������ � ����
        string conn = "URI=file:" + Application.dataPath + "/Database.db"; // ���� � ���� ������
        using (SqliteConnection connection = new SqliteConnection(conn))
        {
            connection.Open();

            // ��������, ���������� �� ��� ������������ � ����� ������
            string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Login = @Login";
            using (SqliteCommand checkCommand = new SqliteCommand(checkUserQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@Login", username);
                int userExists = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (userExists > 0)
                {
                    ShowErrorText("A user with that name already exists."); 
                    return;
                }
            }

         
            string insertQuery = "INSERT INTO Users (Login, Password) VALUES (@Login, @Password)";
            using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, connection))
            {
               
                insertCommand.Parameters.AddWithValue("@Login", username);
                insertCommand.Parameters.AddWithValue("@Password", password);
             
                try
                {
                    insertCommand.ExecuteNonQuery();
                    ShowErrorText("Registration is successful!");
                    PlayerPrefs.SetString("CurrentUsername", username);

                    SceneManager.LoadScene(1);
                   
                    loginInput.text = "";
                    passwordInput.text = "";
                    confirmPasswordInput.text = "";

                }
                catch (Exception e)
                {
                    ShowErrorText("Registration error: " + e.Message); 
                }
            }
        }
    }

    // ����� ��� ��������� ������
    bool ValidateInputs(string username, string password, string confirmPassword, out string errorMessage)
    {
        // �������� ����� ������������
        if (username.Length < 6)
        {
            errorMessage = "The user name must contain at least 6 characters.";
            return false;
        }

        // ��� ������ ��������� ���� �� ���� ����� (������� ��� ����������)
        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"[a-zA-Z�-��-�]"))
        {
            errorMessage = "The user name must contain at least one letter.";
            return false;
        }

        // ��� ������ ��������� ���� �� ���� �����
        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"\d"))
        {
            errorMessage = "The user name must contain at least one digit.";
            return false;
        }

        // ��� �� ������ ��������� ������������ �������
        if (System.Text.RegularExpressions.Regex.IsMatch(username, @"[^\w�-��-�]"))
        {
            errorMessage = "The username must not contain @ characters, spaces, or punctuation marks.";
            return false;
        }

        // �������� ������
        if (password.Length < 6)
        {
            errorMessage = "The password must contain at least 6 characters.";
            return false;
        }

        // ������ ������ ��������� ���� �� ���� ����� (������� ��� ����������)
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-zA-Z�-��-�]"))
        {
            errorMessage = "The password must contain at least one letter.";
            return false;
        }

        // ������ ������ ��������� ���� �� ���� �����
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
        {
            errorMessage = "The password must contain at least one digit.";
            return false;
        }

        // ������ �� ������ ��������� ��������
        if (password.Contains(" "))
        {
            errorMessage = "The password must not contain spaces.";
            return false;
        }

        // �������� ���������� �������
        if (password != confirmPassword)
        {
            errorMessage = "The passwords don't match.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    void ShowErrorText(string message)
    {
        ErrorText.text = message;
    }

    public void ClickBack()
    {
        RegistrationMenu.SetActive(false);
        SelectedMenu.SetActive(true);
    }
}
