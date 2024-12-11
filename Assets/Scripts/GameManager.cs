using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;



public delegate void CurrencyChanged();

public class GameManager : Singleton<GameManager>
{
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
    private TMP_Text waveTxt;

    [SerializeField]
    private TMP_Text currencyTxt;

    [SerializeField]
    private GameObject waveBtn;

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

   

    /// <summary>
    /// The current selected tower
    /// </summary>
    private Tower selectedTower;

    private List<Monster> activeMonsters = new List<Monster>();

    /// <summary>
    /// A property for the object pool
    /// </summary>
    public ObjectPool Pool { get; set; }

    public bool WaveActive
    {
        get {
            return activeMonsters.Count > 0;
        }
    }

    /// <summary>
    /// Property for accessing the currency
    /// </summary>
    public int Currency
    {
        get
        {
            return currency;
        }

        set
        {
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
        Lives = 10;
        Currency = 5    ;
	}
	
	// Update is called once per frame
	void Update () {

        HandleEscape();
	}

    /// <summary>
    /// Pick a tower then a buy button is pressed
    /// </summary>
    /// <param name="towerBtn">The clicked button</param>
    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price && !WaveActive)
        {
            //Stores the clicked button
            this.ClickedBtn = towerBtn;

            //Activates the hover icon
            Hover.Instance.Activate(towerBtn.Sprite);
        }

 
    }

    /// <summary>
    /// Buys a tower
    /// </summary>
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

    /// <summary>
    /// Selects a tower by clicking it
    /// </summary>
    /// <param name="tower">The clicked tower</param>
    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)//If we have selected a tower
        {
            //Selects the tower
            selectedTower.Select();
        }

        //Sets the selected tower
        selectedTower = tower;

        //Selects the tower
        selectedTower.Select();

        sellText.text = "+ " + (selectedTower.Price / 2).ToString();

        upgradePanel.SetActive(true);

        
    }

    /// <summary>
    /// Deselect the tower
    /// </summary>
    public void DeselectTower()
    {
        //If we have a selected tower
        if (selectedTower != null)
        {
            //Calls select to deselect it
            selectedTower.Select();
        }

        upgradePanel.SetActive(false);

        //Remove the reference to the tower
        selectedTower = null;

  
    }

    /// <summary>
    /// Handles escape presses
    /// </summary>
    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//if we press escape
        {
            //Deactivate the hover instance
            Hover.Instance.Deactivate();
        }
    }

    public void StartWave()
    {
        wave++;

        waveTxt.text = string.Format("Wave: <color=#84f542>{0}</color>", wave);

        StartCoroutine(SpawnWave());

        waveBtn.SetActive(false);
    }

    /// <summary>
    /// Spawns a wave of monsters
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnWave()
    {
        //Generates the path
        LevelManager.Instance.GeneratePath();

        for (int i = 0; i < wave; i++)
        {
            int monterIndex = 0; //Random.Range(0, 4);

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

            //Requests the monster form the pool
            Monster monster = Pool.GetObject(type).GetComponent<Monster>();

            monster.Spawn(health);

            if (wave % 3 == 0)
            {
                health += 5;
            }

            //Adds the monster to the activemonster list
            activeMonsters.Add(monster);

            yield return new WaitForSeconds(2.5f);
        }

    }

    /// <summary>
    /// Removes a monster from the game
    /// </summary>
    /// <param name="monster">Monster to remove</param>
    public void RemoveMonster(Monster monster)
    {
        //Removes the monster from the active list
        activeMonsters.Remove(monster);

        //IF we don't have more active monsters and the game isn't over, then we need to show the wave button
        if (!WaveActive && !gameOver)
        {
            //Shows the wave button
            waveBtn.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
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

    public void SetTooltipText(string txt)
    {
        statTxt.text = txt;
    }
}
