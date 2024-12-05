using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    /// <summary>
    /// Reference to the animator
    /// </summary>
    private Animator myAnimator;

	// Use this for initialization
	void Start () {

        //Creates the reference to the animator component
        myAnimator = GetComponent<Animator>();
	}

    /// <summary>
    /// Opens the portal
    /// </summary>
    public void Open()
    {
        //Triggers the open animation
        myAnimator.SetTrigger("Open");
    }
}
