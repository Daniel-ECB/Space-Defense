using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    // Explosion field
    public GameObject prefabExplosion;

    // Called on a 2D physics collision (because the projectile colliders are marked as triggers) 
    void OnTriggerEnter2D(Collider2D other)
    {

        // if the collision is between a player projectile and a robot, destroy both game objects
        if (gameObject.tag == "PlayerProjectile" && other.tag == "Enemy")
        {
            // in if body: create explosion at robot and destroy both objects
            GameObject tempExplosion = Instantiate(prefabExplosion, other.gameObject.transform.position,
                Quaternion.identity) as GameObject;

            DestroyObject(gameObject);
            DestroyObject(other.gameObject);

            // Updating score
            GameObject scoreGameController = GameObject.Find("GameController");
			scoreGameController.SendMessage("AddPoints", GameConstants.ROBOT_POINTS);
			scoreGameController.SendMessage("UpdateRemainingRobots");
            scoreGameController.SendMessage("CheckGameOver");
        }

        // else if the collision is between an enemy projectile and the player
        else if (gameObject.tag == "EnemyProjectile" && other.tag == "Player")
        {
            // in else if body: create explosion at projectile, destroy projectile, and reduce player health
//            GameObject tempExplosion = Instantiate(prefabExplosion, gameObject.transform.position,
//                Quaternion.identity) as GameObject;
            DestroyObject(gameObject);

            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            Player player = playerGameObject.GetComponent<Player>();
            player.Health = player.Health - GameConstants.ROBOT_BEAM_PROJECTILE_DAMAGE;

			GameObject scoreGameController = GameObject.Find("GameController");			
			scoreGameController.SendMessage("CheckGameOver");
        }
    }

    // The projectile is destroyed after leaving screen
    void OnBecameInvisible()
    {
        // destroy the game object
        DestroyObject(gameObject);
    }
}
