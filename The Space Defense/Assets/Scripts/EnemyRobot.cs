using UnityEngine;
using System.Collections;

public class EnemyRobot : MonoBehaviour {

	#region Fields

	// Movement variables
	Rigidbody2D robotRB2D;
	float robotSide;
	[HideInInspector] public bool facingRight = true;
	Vector3 theScale; // Declaration was originally in the Flip function
	Vector2 initialForce;
	
	float elapsedFacingTime = 0;
	float facingDelay;

	// shooting support
	float elapsedShotTime = 0;
	float firingDelay;

	// Robot Projectile Shooting Variables
	Vector3 robotPositionNow;
	
	public GameObject prefabProjectile;
	GameObject newRobotProjectile;
	Rigidbody2D projectileRB;
	Vector2 projectileForce;

	// Animation variables
	Animator animator;

    // sound effects support
    //AudioSource bounceSound;
    AudioSource shootSound;

    #endregion

    #region Private Methods

    // Use this for initialization
    void Start () 
	{
		// Getting the references		
		robotRB2D = GetComponent<Rigidbody2D>();
		robotSide = Mathf.Sign(robotRB2D.position.x); // on which side of the screen the robot is

		animator = GetComponent<Animator> ();

        // save audio sources
        AudioSource[] audioSources = gameObject.GetComponents<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.clip.name == "TeddyBounce")
            {
                //bounceSound = audioSource;
            }
            else if (audioSource.clip.name == "laser4")
            {
                shootSound = audioSource;
            }
        }

            // Moving and facing in the same direction
            if (robotSide > 0 && facingRight)
		{
			Flip();
		}
		else if (robotSide < 0 && !facingRight)
		{
			Flip();
		}

		// Setting some movement and firing variables
		initialForce = new Vector2(GameConstants.ROBOT_MAX_INITIAL_FORCE * -robotSide, 0);
		firingDelay = GetRandomFiringDelay();
		facingDelay = GetRandomFacingDelay();
	}
	
	// FixedUpdate is called once per Physics Update
	void FixedUpdate ()
	{
		// Just moving the robot
		robotRB2D.AddForce(initialForce, ForceMode2D.Impulse);     
		
		// Every once in a while the enemy will change the direction it is facing to 
		elapsedFacingTime += Time.deltaTime;
		
		if (elapsedFacingTime > facingDelay)
		{
			// in if body: reset elapsed facing time and get new random facing delay
			elapsedFacingTime = 0;
			facingDelay = GetRandomFacingDelay();
			
			// Changing movement
			Flip();
		}
	}

	// Update is called once per frame
	void Update () 
	{
		// fire projectile as appropriate
		elapsedShotTime += Time.deltaTime;
		
		if (elapsedShotTime > firingDelay)
		{
			Fire();
		}
	}

	/// <summary>
	/// Function for flipping the sprite
	/// </summary>
	void Flip()
	{
		facingRight = !facingRight;
		theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		initialForce *= -1;
	}
		
	/// <summary>
	/// Gets a random firing delay between MIN_FIRING_DELAY and
	/// MIN_FIRING_DELY + FIRING_RATE_RANGE
	/// </summary>
	/// <returns>the random firing delay</returns>
	private float GetRandomFiringDelay()
	{
		return Random.Range(GameConstants.ROBOT_MIN_FIRING_DELAY,
		                    GameConstants.ROBOT_MIN_FIRING_DELAY + GameConstants.ROBOT_FIRING_RATE_RANGE);
	}

	/// <summary>
	/// Gets a random firing delay between ROBOT_MIN_FACING_DELAY and
	/// ROBOT_MIN_FACING_DELAY + ROBOT_FACING_DELAY_RANGE
	/// </summary>
	/// <returns>the random facing delay</returns>
	private float GetRandomFacingDelay()
	{
		return Random.Range(GameConstants.ROBOT_MIN_FACING_DELAY,
		                    GameConstants.ROBOT_MIN_FACING_DELAY + GameConstants.ROBOT_FACING_DELAY_RANGE);
	}

	/// <summary>
	/// Makes the robot fire
	/// </summary>
	private void Fire()
	{
		// in if body: reset elapsed shot time and get new random firing delay
		elapsedShotTime = 0;
		firingDelay = GetRandomFiringDelay();
		
		// in if body: create and place projectile
		newRobotProjectile = Instantiate(prefabProjectile) as GameObject;
		robotPositionNow = transform.position;
		robotPositionNow.y += GameConstants.ROBOT_PROJECTILE_OFFSET_Y;
		robotPositionNow.x += GameConstants.ROBOT_PROJECTILE_OFFSET_X * Mathf.Sign(theScale.x);
		newRobotProjectile.transform.position = robotPositionNow;
		
		// in if body: shoot projectile down
		projectileRB = newRobotProjectile.GetComponent<Rigidbody2D>();
		projectileForce = new Vector2(GameConstants.ROBOT_PROJECTILE_INITIAL_FORCE * Mathf.Sign(theScale.x), 0);
		projectileRB.AddForce(projectileForce, ForceMode2D.Impulse);
		
		// And of course, playing the animation and sound effect
		animator.SetTrigger("Firing");
        shootSound.Play();
    }
	
	#endregion

	#region Public Methods

	/// <summary>
	/// Fires the shot
	/// </summary>
	public void FireShot()
	{
		Fire ();
	}

	/// <summary>
	/// If the robot hits a safety block, it changes direction.
	/// </summary>
	public void ChangeDirection()
	{
		Flip ();
		GetRandomFacingDelay ();
	}

	#endregion
}
