using UnityEngine;
using System.Collections;

public class Pickups : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		// The Player recovers 10 Points of health when using a pickup
		if (other.tag == "Player") 
		{
			// Debug.Log ("Player Receives Health");
			GameObject player = GameObject.Find("Player");
			Player playerScript = player.GetComponent<Player>();
			playerScript.Health += 10;

			Destroy(gameObject);

//			if(player != null)
//				Debug.Log ("Player Receives Health");
		}
	}
}
