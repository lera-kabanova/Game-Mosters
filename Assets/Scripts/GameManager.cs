using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading;
using Mono.Data.Sqlite;

public delegate void CurrencyChanged();

public class GameManager : Singleton<GameManager>
{

    public TMP_Text KilledMonsters;


    public TMP_Text Dollars;


    public int totalMonstersKilled = 0;
    public int totalCurrencyEarned = 0;

    public event CurrencyChanged Changed;
 
    public TowerBtn ClickedBtn { get; set; }

    private int currency;

    private int wave = 0;

    private int lives;

    private bool gameOver = false;

    private int health = 15;

    [SerializeField]
    private TMP_Text livesTxt;

    [SerializeField]
    private TMP_Text upgradePrice;

    [SerializeField]
    private TMP_Text waveTxt;

    [SerializeField]
    private TMP_Text currencyTxt;

    [SerializeField]
    private GameObject waveBtn;

    [SerializeField]
    private GameObject LevelMenu;

    public float selectedSpeed = 1f;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject upgradePanel;

    [SerializeField]
    private GameObject statsPanel;

    [SerializeField]
    private TMP_Text sellText;


    [SerializeField]
    private Text statTxt;

    [SerializeField]
    private GameObject inGameMenu;

    [SerializeField]
    private GameObject optionsMenu;

    private Tower selectedTower;

    private List<Monster> activeMonsters = new List<Monster>();

    private bool isLevelDifficultySelected = false;
    public ObjectPool Pool { get; set; }

    public bool WaveActive
    {
        get {
            return activeMonsters.Count > 0;
        }
    }

    public int Currency
    {
        get
        {
            return currency;
        }

        set
        {

            if (value > currency)
            {
                totalCurrencyEarned += value - currency; // Добавляем разницу
            }

            this.currency = value;
            this.currencyTxt.text = value.ToString() + " <color=#84f542>$</color>";
            OnCurrencyChanged();
        }
    }

    public int Lives
    {
        get
        {
            return lives;
        }
        set
        {
            this.lives = value;

            if (lives <= 0)
            {
                this.lives = 0;
                GameOver();
            }

            livesTxt.text = lives.ToString();


        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    // Use this for initialization
    void Start ()
    {
        Lives = 1;
        Currency = 50   ;
        totalMonstersKilled = 0; // Сброс счётчика убитых монстров
        totalCurrencyEarned = 0;
    }
	
	// Update is called once per frame
	void Update () {

        HandleEscape();
       
	}

    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price && !WaveActive)
        {
            this.ClickedBtn = towerBtn;

            Hover.Instance.Activate(towerBtn.Sprite);
        }

 
    }

    public void BuyTower()
    {
        if (Currency >= ClickedBtn.Price)
        {
            Currency -= ClickedBtn.Price;

            Hover.Instance.Deactivate();
        }
        
    }

    public void OnCurrencyChanged()
    {
        if(Changed != null)
        {
            Changed();
        }
    }

    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }

        selectedTower = tower;

        selectedTower.Select();

        sellText.text = "+ " + (selectedTower.Price / 2).ToString() + " $";

        upgradePanel.SetActive(true);

        
    }


    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }

        upgradePanel.SetActive(false);

        selectedTower = null;

  
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (LevelMenu.activeSelf) // Если LevelMenu активно
            {
                LevelMenu.SetActive(false); // Скрываем LevelMenu
                inGameMenu.SetActive(true);
            }
            else if (inGameMenu.activeSelf) // Если меню паузы активно
            {
                inGameMenu.SetActive(false); // Скрываем меню паузы
                LevelMenu.SetActive(true); // Показываем LevelMenu
            }
            else if (selectedTower == null && !Hover.Instance.IsVisible) // Если ни башня, ни наведение не активно
            {
                ShowIngameMenu(); // Показываем меню паузы
            }
            else if (Hover.Instance.IsVisible) // Если пользователь что-то выбирает
            {
                DropTower(); // Убираем выбор
            }
            else if (selectedTower != null) // Если башня выбрана
            {
                DeselectTower(); // Снимаем выбор
            }
        }
    }

    public void ShowWaveSpeedMenu()
    {
        LevelMenu.SetActive(true);
        waveBtn.SetActive(false);
    }

    public void SelectSlowSpeed()
    {
        selectedSpeed = 0.5f;
        EnableStartWaveButton();
        LevelMenu.SetActive(false);
        isLevelDifficultySelected = true;  // После выбора сложности

    }

    public void SelectNormalSpeed()
    {
        selectedSpeed = 1f;
        EnableStartWaveButton();
        LevelMenu.SetActive(false);
        isLevelDifficultySelected = true;  // После выбора сложности

    }

    public void SelectFastSpeed()
    {
        selectedSpeed = 1.4f;
        EnableStartWaveButton();
        LevelMenu.SetActive(false);
        isLevelDifficultySelected = true;  // После выбора сложности

    }

    private void EnableStartWaveButton()
    {
        waveBtn.SetActive(true);
    }

    public void StartWaveWithSelectedSpeed()
    {
        LevelMenu.SetActive(false); 
        StartWave(selectedSpeed); 
    }


    public void StartWave(float speedMultiplier)
    {
        foreach (Monster monster in activeMonsters)
        {
            monster.Speed = monster.MaxSpeed * speedMultiplier; 
        }

        wave++;
        waveTxt.text = string.Format("Wave: <color=#84f542>{0}</color>", wave);

        StartCoroutine(SpawnWave());
        waveBtn.SetActive(false);
    }

    private IEnumerator SpawnWave()
    {
        int monstersToSpawn = Mathf.Min(wave, 2);
        LevelManager.Instance.GeneratePath();

        for (int i = 0; i < wave; i++)
        {
            int monterIndex = Random.Range(0, 4);

            string type = string.Empty;

            switch (monterIndex)
            {
                case 0:
                    type = "BlueMonster";
                    break;
                case 1:
                    type = "RedMonster";
                    break;
                case 2:
                    type = "GreenMonster";
                    break;
                case 3:
                    type = "PurpleMonster";
                    break;
            }

            // Получаем монстра из пула
            Monster monster = Pool.GetObject(type).GetComponent<Monster>();

            // Обновляем параметры монстра
            monster.Spawn(health);
            monster.Speed = monster.MaxSpeed * selectedSpeed; // Устанавливаем корректную скорость

            // Увеличиваем здоровье каждые 3 волны
            if (wave % 3 == 0)
            {
                health += 5;
            }

            activeMonsters.Add(monster);

            yield return new WaitForSeconds(2.5f);
        }
    }

    public void RemoveMonster(Monster monster)
    {
        // Удаляем монстра из активного списка
        activeMonsters.Remove(monster);
        
        // Возвращаем скорость монстра для повторного использования
        monster.Speed = monster.MaxSpeed; // Сбрасываем скорость до стандартной

        if (!WaveActive && !gameOver)
        {
            waveBtn.SetActive(true);
        }
    }

    
    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
            KilledMonsters.text = $"Monsters killed: {totalMonstersKilled}";
            Dollars.text = $" Total money: {totalCurrencyEarned}<color=#84f542>$</color>";
            SaveStatisticsToDatabase();
        }
    }

    private void SaveStatisticsToDatabase()
    {
        string username = PlayerPrefs.GetString("CurrentUsername", "UnknownUser"); // Получаем имя текущего пользователя

        string conn = "URI=file:" + Application.dataPath + "/Database.db";
        using (SqliteConnection connection = new SqliteConnection(conn))
        {
            connection.Open();

            // Проверяем, существует ли пользователь
            string checkUserExistsQuery = "SELECT COUNT(*) FROM UserStatistics WHERE Username = @Username";
            using (SqliteCommand checkCommand = new SqliteCommand(checkUserExistsQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@Username", username);
                long count = (long)checkCommand.ExecuteScalar();

                if (count > 0)
                {
                    // Если запись существует, добавляем текущие значения к существующей статистике
                    string updateStatsQuery = @"
                    UPDATE UserStatistics 
                    SET KilledMonsters = KilledMonsters + @KilledMonsters, 
                        EarnedMoney = EarnedMoney + @EarnedMoney
                    WHERE Username = @Username";

                    using (SqliteCommand updateCommand = new SqliteCommand(updateStatsQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Username", username);
                        updateCommand.Parameters.AddWithValue("@KilledMonsters", totalMonstersKilled);
                        updateCommand.Parameters.AddWithValue("@EarnedMoney", totalCurrencyEarned);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Если записи нет, создаём её
                    string insertStatsQuery = @"
                    INSERT INTO UserStatistics (Username, KilledMonsters, EarnedMoney) 
                    VALUES (@Username, @KilledMonsters, @EarnedMoney)";

                    using (SqliteCommand insertCommand = new SqliteCommand(insertStatsQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", username);
                        insertCommand.Parameters.AddWithValue("@KilledMonsters", totalMonstersKilled);
                        insertCommand.Parameters.AddWithValue("@EarnedMoney", totalCurrencyEarned);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            // Обновляем таблицу рекордов
            string upsertLeaderboardQuery = @"
            INSERT OR REPLACE INTO Leaderboard (Username, EarnedMoney) 
            SELECT Username, EarnedMoney 
            FROM UserStatistics
            WHERE Username = @Username";

            using (SqliteCommand leaderboardCommand = new SqliteCommand(upsertLeaderboardQuery, connection))
            {
                leaderboardCommand.Parameters.AddWithValue("@Username", username);
                leaderboardCommand.ExecuteNonQuery();
            }
        }
    }




    public void Restart()
    {
        

        // Сбрасываем текущую игровую статистику
        totalMonstersKilled = 0;
        totalCurrencyEarned = 0;
        wave = 0;
        health = 15;
        Lives = 1;
        Currency = 50;
        LevelMenu.SetActive(true);
        inGameMenu.SetActive(false);
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SaveStatisticsToDatabase();

    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            Currency += selectedTower.Price / 2;

            selectedTower.GetComponentInParent<TileScript>().IsEmpty = true;

            Destroy(selectedTower.transform.parent.gameObject);

            DeselectTower();
        }
    }

    public void ShowStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ShowSelectedTowerStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
        UpdateUpgradeTip();
    }
    public void SetTooltipText(string txt)
    {
        statTxt.text = txt;
    }

    public void UpdateUpgradeTip()
    {
        if (selectedTower != null)
        {
            Debug.Log("update");
            sellText.text = "+ " + (selectedTower.Price / 2).ToString() + " $";
            SetTooltipText(selectedTower.GetStats());

            if (selectedTower.NextUpgrade != null)
            {
                upgradePrice.text = selectedTower.NextUpgrade.Price.ToString() + " $";
            }
            else
            {
                upgradePrice.text = string.Empty;
            }
        }
    }

    public void UpgradeTower()
    {
        if (selectedTower != null)
        {
            if (selectedTower.Level <= selectedTower.Upgrades.Length && Currency >= selectedTower.NextUpgrade.Price)
            {
                selectedTower.Upgrade();
            }
        }
    }

    public void ShowIngameMenu()
    {
        if (optionsMenu.activeSelf)
        {
            ShowMain();
        }
        else
        {
            if (!isLevelDifficultySelected) // Если уровень сложности еще не выбран
            {
                inGameMenu.SetActive(false);
                LevelMenu.SetActive(true); // Показываем меню выбора уровня сложности
            }
            else
            {
                inGameMenu.SetActive(!inGameMenu.activeSelf);
                if (!inGameMenu.activeSelf)
                {
                    Time.timeScale = 1; // Возвращаем нормальную скорость игры
                }
                else
                {
                    Time.timeScale = 0; // Пауза в игре
                }
            }
        }
    }


    private void DropTower()
    {
        ClickedBtn = null;
        Hover.Instance.Deactivate();
    }

    public void ShowOptions()
    {
        inGameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ShowMain()
    {
        inGameMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public GameObject btnShowLeaderboard;  // Кнопка для отображения таблицы
    public GameObject leaderboardImage;  // Используем Image для отображения таблицы
    public Transform leaderboardContent;  // Контейнер для элементов списка
    public GameObject leaderboardItemPrefab;  // Префаб элемента списка

    // Строка подключения к базе данных
    private string connectionString = "Data Source=DataBase.db";
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
