using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class Monster : MonoBehaviour
{

    [SerializeField]
    private float speed;

    private Stack<Node> path;

    protected Animator myAnimator;

    public Point GridPosition { get; set; }

    public bool IsActive { get; set; }

    private Vector3 destination;

    [SerializeField]
    private Stat health;

    private void Awake()
    {
        //Sets up references to the components
        myAnimator = GetComponent<Animator>();
        health.Initialize();
    }

    private void Update()
    {
        Move();
    }

    public void Spawn(int health)
    {
        transform.position = LevelManager.Instance.BluePortal.transform.position;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 1; // Set the order in layer to 1
        }
        this.health.MaxVal = health;
        this.health.CurrentValue = this.health.MaxVal;

        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1), false));

        SetPath(LevelManager.Instance.Path);
    }


    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {
        //The scaling progress
        float progress = 0;

        //As long as the progress is les than 1, then we need to keep scaling
        while (progress <= 1)
        {
            //Scales themonster
            transform.localScale = Vector3.Lerp(from, to, progress);
            progress += Time.deltaTime;
            yield return null;
        }

        //Makes sure that is has the correct scale after scaling
        transform.localScale = to;

        IsActive = true;

        if (remove)
        {
            Release();
        }
    }

    /// <summary>
    /// Makes the unity move along the given path
    /// </summary>
    public void Move()
    {
        if (IsActive)
        {
            //Move the unit towards the next destination
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            //Checks if we arrived at the destination
            if (transform.position == destination)
            {
                //If we have a path and we have more nodes, then we need to keep moving
                if (path != null && path.Count > 0)
                {
                    //Animates the Unit based on the gridposition
                    Animate(GridPosition, path.Peek().GridPosition);

                    //Sets the new gridPosition
                    GridPosition = path.Peek().GridPosition;

                    //Sets a new destination
                    destination = path.Pop().WorldPosition;

                }
            }
        }

    }

    /// <summary>
    /// Gives the Unit a path to walk on
    /// </summary>
    /// <param name="newPath">The unit's new path</param>
    /// <param name="active">Indicates if the unit is active</param>
    public void SetPath(Stack<Node> newPath)
    {
        if (newPath != null) //If we have a path
        {
            //Sets the new path as the current path
            this.path = newPath;

            //Animates the Unit based on the gridposition
            Animate(GridPosition, path.Peek().GridPosition);

            //Sets the new gridPosition
            GridPosition = path.Peek().GridPosition;

            //Sets a new destination
            destination = path.Pop().WorldPosition;
        }
    }

    /// <summary>
    /// Animates the Unit according to the current action
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="newPos"></param>
    public void Animate(Point currentPos, Point newPos)
    {
        //The code below animates the unit based on the moving direction

        //If we are moving down
        if (currentPos.Y > newPos.Y)
        {
            myAnimator.SetInteger("Horizontal", 0);

            myAnimator.SetInteger("Vertical", 1);
        }
        //IF we are moving up
        else if (currentPos.Y < newPos.Y)
        {
            myAnimator.SetInteger("Horizontal", 0);
            myAnimator.SetInteger("Vertical", -1);
        }
        //If we aren't moving up or down
        if (currentPos.Y == newPos.Y)
        {
            //If we are moving left
            if (currentPos.X > newPos.X)
            {
                myAnimator.SetInteger("Vertical", 0);
                myAnimator.SetInteger("Horizontal", -1);
            }
            //If we are moving right
            else if (currentPos.X < newPos.X)
            {
                myAnimator.SetInteger("Vertical", 0);
                myAnimator.SetInteger("Horizontal", 1);
            }
        }
    }

    /// <summary>
    /// When the monster collides with something
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "RedPortal")//If we collide with the red portal
        {
            //Start scaling the monster down
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));

            //Plays the portal animation
            other.GetComponent<Portal>().Open();

            GameManager.Instance.Lives--;
        }
    }

    /// <summary>
    /// Releases a monster from the game into the object pool
    /// </summary>
    private void Release()
    {
        //Makes sure that it isn't active
        IsActive = false;

        //Makes sure that it has the correct start position
        GridPosition = LevelManager.Instance.BlueSpawn;

        //Removes the monster from the game
        GameManager.Instance.RemoveMonster(this);

        //Releases the object in the object pool
        GameManager.Instance.Pool.ReleaseObject(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if(IsActive)
        {
            health.CurrentValue -= damage;   
        }
    }
}
