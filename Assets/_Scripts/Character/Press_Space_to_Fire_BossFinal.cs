﻿using UnityEngine;
using System.Collections;


public class Press_Space_to_Fire_BossFinal : MonoBehaviour
{
	//Drag in the Bullet Emitter from the Component Inspector.
	public GameObject Bullet_Emitter;

	public float bulletdirection;

	public AudioSource audiosource;
	public AudioClip gunfire;

	public Transform Character;
	private Vector3 directionOfCharacter;

	private Rigidbody Villain;

	//Drag in the Bullet Prefab from the Component Inspector.
	public GameObject Bullet;

	public Vector3 move;
	public float Delay;

	public static bool crouch;

	public Animator m_Animator;

	public static bool fire;
	public float distance;

	private float m_CapsuleHeight;
	private Vector3 m_CapsuleCenter;
	public static BoxCollider m_Capsule;

	//Enter the Speed of the Bullet from the Component Inspector.
	public float Bullet_Forward_Force;

	public static bool canshoot;

	// Use this for initialization
	void Start ()
	{
		fire = false;
		crouch = false;
		m_Capsule = GetComponent<BoxCollider>();
		m_CapsuleHeight = m_Capsule.size.y;
		m_CapsuleCenter = m_Capsule.center;
		m_Animator = GetComponent<Animator>();
		Villain = GetComponent<Rigidbody>();
		bulletdirection = 90;
		audiosource = GetComponent<AudioSource> ();
		Delay = 1;
	}

	// Update is called once per frame
	void Update ()
	{
		Delay -= Time.deltaTime;
		Vector3 target = new Vector3 (Character.position.x, Villain.position.y, Character.position.z);
		if (!m_Animator.GetBool ("Death") && canshoot && (Delay <= 0)) {
			
			Villain.transform.LookAt (target);

			//The Bullet instantiation happens here.
			GameObject Temporary_Bullet_Handler;
			Vector3 tempPosition = new Vector3 (72, Bullet_Emitter.transform.position.y, Bullet_Emitter.transform.position.z);
			Temporary_Bullet_Handler = Instantiate (Bullet, tempPosition, Bullet_Emitter.transform.rotation) as GameObject;


//				Temporary_Bullet_Handler.transform.Rotate(Vector3.left * bulletdidddddddrection);
			//Retrieve the Rigidbody component from the instantiated Bullet and control it.
			Rigidbody Temporary_RigidBody;
			Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody> ();

			directionOfCharacter = Character.transform.position - new Vector3 (Bullet_Emitter.transform.position.x, Bullet_Emitter.transform.position.y - 1, Bullet_Emitter.transform.position.z);
			directionOfCharacter = directionOfCharacter.normalized;

			Temporary_RigidBody.AddForce (directionOfCharacter * Bullet_Forward_Force);

			Temporary_RigidBody.position = new Vector3 (Character.position.x, Temporary_RigidBody.position.y, Temporary_RigidBody.position.z);

			audiosource.PlayOneShot (gunfire);

			fire = false;


			//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
			Destroy (Temporary_Bullet_Handler, 10f);
			m_Animator.SetBool ("Fire", false);
			Delay = 0.2f;
		}
	}
}