﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour {

	public float runSpeed = 3f;
	public float walkSpeed = 1.5f;
	public float tackleSpeed = 10f;

	private CharacterController controller;
	private Transform cameraTransform;
	private Animator anim;
	private Vector3 moveVector;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponent <Animator> ();

	}
	
	// Update is called once per frame
	void Update () {

		float speed;

		if (EnergyBarScript.energy < 100)
			EnergyBarScript.energy++;

		if (Input.GetKeyDown (KeyCode.Space) && (EnergyBarScript.energy == 100)) {
			speed = tackleSpeed;
			anim.SetTrigger ("Tackle");

		}


		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			anim.SetTrigger ("Jump");

		}

		moveVector = Vector3.zero;
		speed = (Input.GetKey (KeyCode.LeftShift) ? runSpeed : walkSpeed);

		moveVector.z = Input.GetAxis ("Horizontal");


		controller.Move (moveVector * (speed * (EnergyBarScript.energy / 100f)) * Time.deltaTime);

		if (controller.velocity != Vector3.zero)
			transform.forward = controller.velocity;

		anim.SetFloat ("Speed", controller.velocity.magnitude);
	}

	private Vector3 RotateWithView () {
		if (cameraTransform != null) {
			Vector3 dir = cameraTransform.TransformDirection (moveVector);
			dir.Set (dir.x, 0, dir.z);
			return dir.normalized * moveVector.magnitude;
		} else {
			cameraTransform = Camera.main.transform;
			return moveVector;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("PickUp")) {
			other.gameObject.SetActive(false);
		}
	}
}
