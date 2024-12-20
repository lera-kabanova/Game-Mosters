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
    private TMP_InputField loginInput;    // ���� ������

    [SerializeField]
    private TMP_InputField passwordInput;    // ���� ������

    [SerializeField]
    private TMP_Text ErrorLogin; // ����� ��� ��������� �� ������� ��� ������

    void Start()
    {
        // ����������� ���� ����� ��� ����������� ������� ��� ���������
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
        
        
    }

    // ����� ��� ����� ������������
    public void LoginUser()
    {
        string username = loginInput.text.Trim();
        string password = passwordInput.text.Trim();

        // �������� �����
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowErrorText("Please fill in all fields.");
            return;
        }

        // �������� ������ � ����
        string conn = "URI=file:" + Application.dataPath + "/Database.db"; // ���� � ���� ������
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
                        // �������� ����
                        ShowErrorText("Login successful!");
                        SceneManager.LoadScene(1); // �������� �� ������ �����
                        PlayerPrefs.SetString("CurrentUsername", username);

                    }
                    else
                    {
                        // �������� ����� ��� ������
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
