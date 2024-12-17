using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Для работы со сценами
using TMPro;

public class SelectedMenuManager : MonoBehaviour
{
    public SqliteConnection dbConnection;

    [SerializeField]
    private GameObject SelectedMenu;

    [SerializeField]
    private GameObject RegisterMenu;

    [SerializeField]
    private GameObject LoginMenu;

    [SerializeField]
    private GameObject Register;

    [SerializeField]
    private GameObject Login;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void RegisterClick()
    {
        SelectedMenu.SetActive(false);
        RegisterMenu.SetActive(true);
       // SceneManager.LoadScene(1);
    }

    public void LoginClick()
    {

        SelectedMenu.SetActive(false);
        LoginMenu.SetActive(true);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
