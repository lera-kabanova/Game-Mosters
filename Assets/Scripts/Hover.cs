using UnityEngine;

public class Hover : Singleton<Hover>
{
    private SpriteRenderer spriteRenderer;

    private SpriteRenderer rangeSpriteRenderer;

    public bool IsVisible { get; private set; }

	// Use this for initialization
	void Start ()
    {
        //Creates the references to the sprite renderer
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        this.rangeSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        FollowMouse();
	}

    private void FollowMouse()
    {
        if (spriteRenderer.enabled)
        {
            //Translates the mouse on screen position into a world position
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Resets the Z value to 0
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

    }
        
    public void Activate(Sprite sprite)
    {
        //Sets the correct sprite
        this.spriteRenderer.sprite = sprite;

        //Enables the renderer
        spriteRenderer.enabled = true;

        rangeSpriteRenderer.enabled = true;
        IsVisible= true;
    }

    /// <summary>
    /// Deactivates the hover icon (hides it)
    /// </summary>
    public void Deactivate()
    {
        //Disables the renderer so that we cant see it
        spriteRenderer.enabled = false;

        rangeSpriteRenderer.enabled = false;

        //Unclicks our button
        GameManager.Instance.ClickedBtn = null;
        IsVisible= false;

  
    }
}
