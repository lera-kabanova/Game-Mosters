using UnityEngine;

public class Hover : Singleton<Hover>
{
    /// <summary>
    /// A reference to the icon's spriterenderer
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// A referenceo to the rangedcheck on the tower
    /// </summary>
    private SpriteRenderer rangeSpriteRenderer;

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
        //Makes sure that we follow the mouse
        FollowMouse();
	}

    /// <summary>
    /// Makes the hover icon follow the mouse
    /// </summary>
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

    /// <summary>
    /// Activates the hover icon
    /// </summary>
    /// <param name="sprite">The sprite to show on the hover icon</param>
    public void Activate(Sprite sprite)
    {
        //Sets the correct sprite
        this.spriteRenderer.sprite = sprite;

        //Enables the renderer
        spriteRenderer.enabled = true;

        rangeSpriteRenderer.enabled = true;
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

  
    }
}
