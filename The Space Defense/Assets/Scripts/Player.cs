using UnityEngine;
using System.Collections;
using UnityEngine.UI; // For getting access to UI functions

public class Player : MonoBehaviour {

	#region Fields

	// General References
	public GameController controllerReference;

	// Physical and animation Variables
	private bool grounded = true;
	private bool onTopOfEnemy = false;
	private Rigidbody2D rb2d;
	private BoxCollider2D playerBoxCollider;
	private CircleCollider2D playerCircleCollider;
	
	private Animator anim;
	private GameObject lowerBody;
	private GameObject upperBody;
	private Animator lowerBodyAnim;
	public GameObject prefabExplosion;

	// Player Movement Variables
	[HideInInspector] public bool facingRight = true;

	float playerX;
	float playerSpeedX = 0.05f;
	Vector3 theScale;
	float playerHorDir;

	[HideInInspector] public bool jump = false;
	public float jumpForce = 0f;
	public Transform groundCheckStart;
	public Transform groundCheckEnd;

	// Player Shooting Variables
	Vector3 weaponPositionNow;
	bool canShoot = true;
	public GameObject beamType;
	GameObject newProjectile;
	Rigidbody2D projectileRB;
	Vector2 projectileForce;

	// GUI Support
	public int health;
	public Text healthText;
	public Slider healthSlider;

	bool playerDead = false;

    // sound effects support
    AudioSource damageSound;
    AudioSource deathSound;
    AudioSource shootSound;

    public GameObject playerDeathSound;

    #endregion

    #region Private Methods

    // Use this for initialization
    void Start () 
	{
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		health = GameConstants.PLAYER_HEALTH;

		playerBoxCollider = GetComponent<BoxCollider2D>();
		playerCircleCollider = GetComponent<CircleCollider2D>();

		lowerBody = transform.Find ("Player_Lower").gameObject;
		upperBody = transform.Find ("Player_Upper").gameObject;
		lowerBodyAnim = lowerBody.GetComponent<Animator>();

        // save audio sources
        AudioSource[] audioSources = gameObject.GetComponents<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.clip.name == "pain1")
            {
                damageSound = audioSource;
            }
            else if (audioSource.clip.name == "die2")
            {
                deathSound = audioSource;
            }
            else if (audioSource.clip.name == "laser1")
            {
                shootSound = audioSource;
            }
        }
    }
	
	// FixedUpdate is called once per physics update
	void FixedUpdate () 
	{
		if (controllerReference.HasGameStarted ()) 
		{
			// Moving the Player
			playerHorDir = Input.GetAxis ("Horizontal");

			playerX = gameObject.transform.position.x;
			playerX += playerHorDir * playerSpeedX;
			gameObject.transform.position = new Vector3 (playerX, transform.position.y, transform.position.z);

			// Moving and facing in the same direction
			if (playerHorDir > 0 && !facingRight) {
				//return; some debug
				Flip ();
			} else if (playerHorDir < 0 && facingRight) {
				//return; some debug
				Flip ();
			}

			// Setting the walking animation
			// In the end this never worked, I used floats instead of triggers in Update
//		if (/*lowerBodyAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && 
//		    (Input.GetKey ("d") || Input.GetKey ("a"))*/ Input.GetAxis("Horizontal") > 0.01) 
//		{
//			//Debug.Log("Holissss");
//			lowerBodyAnim.SetTrigger("Walking");
//		}
//
//		if (/*lowerBodyAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_Walking") &&*/ 
//		    /*(Input.GetKeyUp ("d") || Input.GetKeyUp ("a")) &&*/ Input.GetAxis("Horizontal") < 0.01) 
//		{
////			Debug.Log("Deja de caminar");
//			lowerBodyAnim.SetTrigger("StopWalking");
//		}

			if (lowerBodyAnim != null) {
				//Find the child named "ammo" of the gameobject "magazine" (magazine is a child of "gun").
				//Debug.Log("Lower animator found");
			}

			if (jump) {
				//anim.SetTrigger("Jump"); // Trigger starts jump animation
				rb2d.AddForce (new Vector2 (0f, jumpForce)); // Jump movement
				jump = false;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(controllerReference.HasGameStarted ())
		{
			// Setting the walking animation for the Player
			playerHorDir = Input.GetAxis("Horizontal");
			lowerBodyAnim.SetFloat("Player_Walking", Mathf.Abs(playerHorDir));
			//Debug.Log ("(Update) playerHorDir absolute value: " + Mathf.Abs(playerHorDir));

			// Making the player shoot
			if (Input.GetButtonDown("Fire1"))
			{
				canShoot = false;
				
				// in if body: create and place projectile
				newProjectile = Instantiate(beamType) as GameObject;
				weaponPositionNow = transform.position;
				weaponPositionNow = new Vector3(
					weaponPositionNow.x + GameConstants.RIFLE_BEAM_PROJECTILE_OFFSET_X * Mathf.Sign(theScale.x),
					weaponPositionNow.y + GameConstants.RIFLE_BEAM_PROJECTILE_OFFSET_Y, 0f);
				newProjectile.transform.position = weaponPositionNow;
				
				// in if body: shoot projectile
				projectileRB = newProjectile.GetComponent<Rigidbody2D>();
				projectileForce = new Vector2(GameConstants.BEAM_PROJECTILE_INITIAL_FORCE * Mathf.Sign(theScale.x), 0f);
				projectileRB.AddForce(projectileForce, ForceMode2D.Impulse);

				if(!facingRight)
				{
					Vector3 playerProjectileScale = projectileRB.transform.localScale;
					playerProjectileScale.x *= -1;

					projectileRB.transform.localScale = playerProjectileScale;
				}
				
				// in if body: play sound effect
				shootSound.Play();
			}
					
			// Making the player Jump
			grounded = Physics2D.Linecast(groundCheckStart.position, groundCheckEnd.position,
			                              1 << LayerMask.NameToLayer("Ground"));
			onTopOfEnemy = Physics2D.Linecast(groundCheckStart.position, groundCheckEnd.position,
			                                  1 << LayerMask.NameToLayer("Enemy"));

			if (Input.GetButtonDown("Jump") && (grounded || onTopOfEnemy))
			{
				jump = true;
				upperBody.GetComponent<SpriteRenderer>().enabled = false;
				//lowerBody.GetComponent<SpriteRenderer>().enabled = false;
				lowerBodyAnim.SetTrigger("Jumping");
				playerBoxCollider.enabled = false;
				playerCircleCollider.enabled = true;
			}

	//		if(lowerBodyAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_Jumping") && grounded)
	//		{
	//			lowerBodyAnim.SetTrigger("StopJumping");
	//			upperBody.GetComponent<SpriteRenderer>().enabled = true;
	//		}

			// Still, making him jump
			if(lowerBodyAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_Jumping") &&
			   lowerBodyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
			{
				//Debug.Log("We are jumping");
				upperBody.GetComponent<SpriteRenderer>().enabled = true;

				playerBoxCollider.enabled = true;
				playerCircleCollider.enabled = false;

				if (Input.GetButtonDown("Fire1"))
				{
					//Debug.Log("Dispara al saltar");
				}
			}
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
	}

    /// <summary>
    /// Eliminates the player
    /// </summary>
    private void KillPlayer()
    {
        DestroyObject(gameObject);
    }

	#endregion

	#region Properties
	
	/// <summary>
	/// Gets and sets the current health of the player
	/// </summary>
	public int Health
	{
		get { return health; }
		set
		{
			if (value < health)
			{
				damageSound.Play();
			}
			
			health = value;
			health = Mathf.Clamp(health, 0, 100);

			// Update GUI
			//healthText = GameObject.Find("Health").GetComponent<Text>();
			healthText.text = /*GameConstants.HEALTH_PREFIX +*/ health.ToString();
			healthSlider.value = health;

			// Check if the player was killed
			if (health == 0 && playerDead == false)
			{
				playerDead = true;
				GameObject tempExplosion = Instantiate(prefabExplosion, gameObject.transform.position,
				                                       Quaternion.identity) as GameObject;
                //deathSound.Play();
                //upperBody.GetComponent<SpriteRenderer>().enabled = false;
                //lowerBody.GetComponent<SpriteRenderer>().enabled = false;
                //Invoke("KillPlayer", 0.35f);
                GameObject tempDeathSound = Instantiate(playerDeathSound, gameObject.transform.position,
                Quaternion.identity) as GameObject;

                DestroyObject(gameObject);				
			}
		}
	}
	
	#endregion
}
