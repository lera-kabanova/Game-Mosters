using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; private set; }

    public bool IsEmpty { get; set; }

    private Tower myTower;

    private Color32 fullColor = new Color32(255, 118, 118, 255);

    private Color32 emptyColor = new Color32(96, 255, 90, 255);

    private SpriteRenderer spriteRenderer;

    public bool Walkable { get; set; }

    public bool Debugging { get; set; }

    public Vector2 WorldPosition
    {
        get
        {
            return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 2));
        }
    }

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        Walkable = true;
        IsEmpty = true;
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
        {
            if (IsEmpty && !Debugging)//Colors the tile green
            {
                ColorTile(emptyColor);
            }
            if (!IsEmpty && !Debugging)//Colors the tile red
            {
                ColorTile(fullColor);
            }
            else if (Input.GetMouseButtonDown(0))//Places a tower if there is room on the tile
            {
                PlaceTower();
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn == null && Input.GetMouseButtonDown(0))
        {
            if (myTower != null)
            {
                GameManager.Instance.SelectTower(myTower);
            }
            else
            {
                GameManager.Instance.DeselectTower();
            }
        }

    }

    private void OnMouseExit()
    {
        if (!Debugging)
        {
            ColorTile(Color.white);
        }

    }

    private void PlaceTower()
    {
        Walkable = false;

        if(AStar.GetPath(LevelManager.Instance.BlueSpawn, LevelManager.Instance.RedSpawn) == null)
        {
            Walkable = true;
            return;
        }
        GameObject tower = (GameObject)Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, transform.position, Quaternion.identity);

        //Set the sorting layer order on the tower
        tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y;

        //Sets the tile as transform parent to the tower
        tower.transform.SetParent(transform);

        this.myTower = tower.transform.GetChild(0).GetComponent<Tower>();

        //Makes sure that it isn't empty
        IsEmpty = false;

        //Sets the color back to white
        ColorTile(Color.white);

        myTower.Price = GameManager.Instance.ClickedBtn.Price;

        //Buys the tower
        GameManager.Instance.BuyTower();

        Walkable = false;
    }
    private void ColorTile(Color newColor)
    {
        //Sets the color on the tile
        spriteRenderer.color = newColor;
    }
}
