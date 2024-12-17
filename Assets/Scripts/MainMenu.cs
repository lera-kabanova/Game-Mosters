using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private GameObject mainMenu;

    public void Options()
    {
        if (optionsMenu.activeSelf)
        {
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
        else
        {
            mainMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }

    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
       
    }

    public void Play()
    {
        SceneManager.LoadScene(2);
    }
}
