using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	// cached for efficiency
	Animator anim;
	
	// Use for initialization
	void Start () 
	{		
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// destroy the game object if the explosion has finished its animation
		if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
		{
			DestroyObject(gameObject);
		}			
	}
}
