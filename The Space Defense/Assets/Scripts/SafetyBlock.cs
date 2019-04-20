using UnityEngine;
using System.Collections;

public class SafetyBlock : MonoBehaviour {
	
	void OnTriggerEnter2D (Collider2D other)
	{
		// If the robot hits a safety block, it changes direction.
		if (other.gameObject.tag == "Enemy")
		{
			//Debug.Log("Safety Block was Hit");
			EnemyRobot theRobot = other.GetComponent<EnemyRobot>();
			theRobot.ChangeDirection();
		}
	}
}
