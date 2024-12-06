using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class Monster : MonoBehaviour
{

    [SerializeField]
    private float speed;

    private Stack<Node> path;

    [SerializeField]
    private Element elementType;

    private SpriteRenderer spriteRenderer;

    private int invulnerability = 2;

    protected Animator myAnimator;

    public Point GridPosition { get; set; }

    public bool IsActive { get; set; }

    [SerializeField]
    private GameObject upgradePanel;

    private Vector3 destination;

    [SerializeField]
    private Stat health;

    public bool Alive
    {
        get
        {
            return health.CurrentValue > 0;
        }
    }

    private void Awake()
    {
     
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health.Initialize();
    }

    private void Update()
    {
        Move();
    }

    public void Spawn(int health)
    {
        transform.position = LevelManager.Instance.BluePortal.transform.position;
        this.health.Bar.Reset();
        
        this.health.MaxVal = health;
        this.health.CurrentValue = this.health.MaxVal;

        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1), false));

        SetPath(LevelManager.Instance.Path);
    }


    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {
       
        float progress = 0;

        while (progress <= 1)
        {
         
            transform.localScale = Vector3.Lerp(from, to, progress);
            progress += Time.deltaTime;
            yield return null;
        }

        transform.localScale = to;

        IsActive = true;

        if (remove)
        {
            Release();
        }
    }

    public void Move()
    {
        if (IsActive)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);


            if (transform.position == destination)
            {
             
                if (path != null && path.Count > 0)
                {
                    
                    Animate(GridPosition, path.Peek().GridPosition);

                    
                    GridPosition = path.Peek().GridPosition;

                   
                    destination = path.Pop().WorldPosition;

                }
            }
        }

    }

    public void SetPath(Stack<Node> newPath)
    {
        if (newPath != null) 
        {
           
            this.path = newPath;

           
            Animate(GridPosition, path.Peek().GridPosition);

          
            GridPosition = path.Peek().GridPosition;

            
            destination = path.Pop().WorldPosition;
        }
    }

    public void Animate(Point currentPos, Point newPos)
    {
        
        if (currentPos.Y > newPos.Y)
        {
            myAnimator.SetInteger("Horizontal", 0);

            myAnimator.SetInteger("Vertical", 1);
        }
      
        else if (currentPos.Y < newPos.Y)
        {
            myAnimator.SetInteger("Horizontal", 0);
            myAnimator.SetInteger("Vertical", -1);
        }
       
        if (currentPos.Y == newPos.Y)
        {
           
            if (currentPos.X > newPos.X)
            {
                myAnimator.SetInteger("Vertical", 0);
                myAnimator.SetInteger("Horizontal", -1);
            }
            
            else if (currentPos.X < newPos.X)
            {
                myAnimator.SetInteger("Vertical", 0);
                myAnimator.SetInteger("Horizontal", 1);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "RedPortal")
        {
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));

            other.GetComponent<Portal>().Open();

            GameManager.Instance.Lives--;
        }

        if(other.tag == "Tile")
        {
           spriteRenderer.sortingOrder = other.GetComponent<TileScript>().GridPosition.Y;
        }
    }


    public void Release()
    {
        IsActive = false;

        GridPosition = LevelManager.Instance.BlueSpawn;

        GameManager.Instance.RemoveMonster(this);

        GameManager.Instance.Pool.ReleaseObject(gameObject);
    }

    public void TakeDamage(int damage, Element dmgSource)
    {
        if(IsActive)
        {
            if(dmgSource == elementType)
            {
                damage = damage / invulnerability;
                invulnerability++;
            }
            health.CurrentValue -= damage;   

            if(health.CurrentValue <= 0 )
            {
                GameManager.Instance.Currency += 2;
                myAnimator.SetTrigger("Die");
                IsActive = false;
                GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }
    }
}
