using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject playBtn;

    public GameObject canvas;
  
    [SerializeField]
    private Canvas loadScreen;

    [SerializeField]
    private Stat loadingStat;

    [SerializeField]
    private Image fill;

    private float fillAmount;

    public bool Loading { get; private set; }

    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    private CameraMovement cameraMovement;

    [SerializeField]
    private Transform map;

    private Point blueSpawn, redSpawn;

    [SerializeField]
    private GameObject bluePortalPrefab;

    [SerializeField]
    private GameObject redPortalPrefab;

    public Portal BluePortal { get; set; }

    private Stack<Node> fullPath;

    private Point mapSize;

    public Dictionary<Point, TileScript> Tiles { get; set; }

    public float TileSize
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    public Stack<Node> Path
    {
        get
        {
            if (fullPath == null)
            {
                GeneratePath();
            }

            return new Stack<Node>(new Stack<Node>(fullPath));
        }
    }

    public Point BlueSpawn
    {
        get
        {
            return blueSpawn;
        }
    }

    public Point RedSpawn
    {
        get
        {
            return redSpawn;
        }
    }

    void Start()
    {
        StartLoading();
    }

    void Update()
    {
        if (Loading)
        {
            UpdateBar();
        }
    }

    public void StartLevel()
    {
        loadScreen.enabled = false;

        Loading = false;
    }

    private void StartLoading()
    {
        loadScreen.enabled = true;
        fillAmount = 0f; 
        fill.fillAmount = fillAmount;

        Loading = true;
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
        CreateLevel();
        canvas.gameObject.SetActive(true);
    }

    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>();

        string[] mapData = ReadLevelText();

        mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);

        int mapX = mapData[0].ToCharArray().Length;

        int mapY = mapData.Length;

        Vector3 maxTile = Vector3.zero;

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));

        for (int y = 0; y < mapY; y++) 
        {
            char[] newTiles = mapData[y].ToCharArray(); 

            for (int x = 0; x < mapX; x++) 
            {
                PlaceTile(newTiles[x].ToString(), x, y, worldStart);
            }
        }

        maxTile = Tiles[new Point(mapX - 1, mapY - 1)].transform.position;

        cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));

        SpawnPortals();

        StartLevel();
    }

    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        int tileIndex = int.Parse(tileType);

        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();

        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), map);
    }

    private string[] ReadLevelText()
    {
        TextAsset bindata = Resources.Load("Level") as TextAsset;

        string data = bindata.text.Replace(Environment.NewLine, string.Empty);

        return data.Split('-');
    }

    private void SpawnPortals()
    {
        blueSpawn = new Point(0, 0);
        GameObject tmp = Instantiate(bluePortalPrefab, Tiles[BlueSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
        BluePortal = tmp.GetComponent<Portal>();
        BluePortal.name = "BluePortal";

        redSpawn = new Point(11, 6);
        Instantiate(redPortalPrefab, Tiles[redSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
    }

    public bool InBounds(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y < mapSize.Y;
    }

    public void GeneratePath()
    {
        fullPath = AStar.GetPath(BlueSpawn, redSpawn);
    }

    private void UpdateBar()
    {
        fill.fillAmount = fillAmount;
    }
}
