﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.

	public float moveForce = 637f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 7f;				// The fastest the player can travel in the x axis.
	//public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 2177f;			// Amount of force added when the player jumps.
	//public AudioClip[] taunts;				// Array of clips for when the player taunts.
	//public float tauntProbability = 50f;	// Chance of a taunt happening.
	//public float tauntDelay = 1f;			// Delay for when the taunt should happen.

	public GameObject explosion;

	public AudioClip jumpClip;
	public AudioClip explosionClip;

	//private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.

	private bool isAlive = true;
	private float lastJump;


	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();
	}


	void Update()
	{
		if (this.isAlive) {
			// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
			grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));  

			// If the jump button is pressed and the player is grounded then the player should jump.
			/*if(Input.GetButtonDown("Jump") && grounded)
			jump = true;*/
			if ((Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) && grounded && (Time.time > lastJump + 0.5f)){
				Debug.Log( lastJump);
				lastJump = Time.time;	
				jump = true;

				GameManager.Instance.PlayClipAtLocation (this.jumpClip, this.transform.position);
			}
		}
	}

	void FixedUpdate ()
	{
		if (this.isAlive) {
			// Cache the horizontal input.
			// float h = Input.GetAxis ("Horizontal");
			float h = 0f;
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
				h -= 1;
			}
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
				h = +1;
			}

			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat ("Speed", Mathf.Abs (h));

			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			if (h * rigidbody2D.velocity.x < maxSpeed)
			// ... add a force to the player.
			rigidbody2D.AddForce (Vector2.right * h * moveForce);

			// If the player's horizontal velocity is greater than the maxSpeed...
			if (Mathf.Abs (rigidbody2D.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody2D.velocity = new Vector2 (Mathf.Sign (rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

			// If the input is moving the player right and the player is facing left...
			if (h > 0 && !facingRight)
			// ... flip the player.
			Flip ();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (h < 0 && facingRight)
			// ... flip the player.
			Flip ();

			// If the player should jump...
			if (jump) {
				// Set the Jump animator trigger parameter.
				anim.SetTrigger ("Jump");

				// Play a random jump audio clip.
				//int i = Random.Range(0, jumpClips.Length);
				//AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

				// Add a vertical force to the player.
				rigidbody2D.AddForce (new Vector2 (0f, jumpForce));

				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				jump = false;
			}
		}
	}


	
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnCollisionEnter2D (Collision2D other){
		if (other.gameObject.layer == LayerMask.NameToLayer("Enemies")){
			this.isAlive = false;

			foreach (Collider2D c in this.GetComponentsInChildren<Collider2D>()){
				c.enabled = false;
			}

			foreach (SpriteRenderer sr in this.GetComponentsInChildren<SpriteRenderer>()){
				sr.enabled = false;
			}

			foreach (Rigidbody2D r in this.GetComponentsInChildren<Rigidbody2D>()){
				r.isKinematic = true;
			}

			if (this.explosion != null) {
				GameObject newExplosion = GameObject.Instantiate (this.explosion, this.transform.position, Quaternion.identity) as GameObject;

				GameObject.Destroy (newExplosion, 0.27f);

				GameManager.Instance.PlayClipAtLocation (this.explosionClip, this.transform.position);

				StartCoroutine ("ResetLevel", 1.0f);
			}
		}
	}

	IEnumerator ResetLevel (float delay){
		float timer = 0.0f;

		while (timer < delay) {
			yield return new WaitForEndOfFrame ();
			timer += Time.deltaTime;
		}

		Application.LoadLevel (Application.loadedLevel);
	}
}
