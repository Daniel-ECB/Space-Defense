using UnityEngine;
using System.Collections;

public class PlayerDetector : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other)
	{
		// If the robot detects the player, he fires
		if(other.gameObject.tag == "Player")
		{
			//Debug.Log("Player has been detedted");
			Transform parentRobotTransform = gameObject.transform.root;
			GameObject parentRobotObject = parentRobotTransform.gameObject;
			EnemyRobot parentRobot = parentRobotObject.GetComponent<EnemyRobot>();

			parentRobot.FireShot();

			//Debug.Log(parentRobotTransform);
		}
	}
}
