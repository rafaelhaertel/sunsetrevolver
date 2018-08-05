using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 3f;
	public Text countText;
	public Text winText;

	Animator anim;

	private Rigidbody rb;
	private int count;

	void Start () {
		
		rb = GetComponent<Rigidbody> ();
		count = 0;
		anim = GetComponent<Animator> ();
		SetCountText ();
		winText.text = "";
	}

	void FixedUpdate () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, rb.velocity.y, moveVertical);

		// Normalise the movement vector and make it proportional to the speed per second.
		movement = movement.normalized * speed * Time.deltaTime;


		// Move the player to it's current position plus the movement.
		rb.MovePosition (transform.position + movement);

		anim.SetFloat ("Speed", speed);

		if (EnergyBarScript.energy < 100)
			EnergyBarScript.energy++;

	}
		
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("PickUp")) {
			other.gameObject.SetActive(false);
			count++;
			SetCountText ();
		}
	}

	void SetCountText () {
		countText.text = "Count: " + count.ToString ();
		if (count >= 8)
			winText.text = "You Win!";
	}

}