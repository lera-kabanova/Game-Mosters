using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private Stat loadingStat;

    [SerializeField]
    private Canvas loadingCanvas; // Экран загрузки

    [SerializeField]
    private Image fill; // Полоса загрузки

    public bool isLoading { get; private set; }
    private float fillAmount;
    void Start()
    {
        StartLoading();
    }
    void Update()
    {
        if (isLoading)
        {
            UpdateBar();
        }
    }
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
        //Application.Quit(); // Завершение приложения
    }

    public void Play()
    {
        SceneManager.LoadScene(2);
    }

    private void StartLoading()
    {
        //loadScreen.enabled = true;
        fillAmount = 0f;
        fill.fillAmount = fillAmount;

        isLoading = true;
        StartCoroutine(LoadingProcess());
    }

    private IEnumerator LoadingProcess()
    {
        float loadingTime = 2f;
        float currentTime = 0f;

        while (currentTime < loadingTime)
        {
            currentTime += Time.deltaTime;
            fillAmount = Mathf.Clamp01(currentTime / loadingTime);
            fill.fillAmount = fillAmount;

            yield return null;
        }
        loadingCanvas.gameObject.SetActive(false);
      
        canvas.gameObject.SetActive(true);

    }
    private void UpdateBar()
    {
        fill.fillAmount = fillAmount;
    }
}
