using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

	private Animator myAnimator;
	// Use this for initialization
	void Start () {
		myAnimator = GetComponent<Animator>();
	
	}
	
	public void Open()
	{
		myAnimator.SetTrigger("Open");
	}
}
