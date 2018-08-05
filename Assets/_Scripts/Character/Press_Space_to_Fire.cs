using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;


namespace UnityStandardAssets.Characters.ThirdPerson1
{
	public class Press_Space_to_Fire : MonoBehaviour
	{
		//Drag in the Bullet Emitter from the Component Inspector.
		public GameObject Bullet_Emitter;

		public float shotangle;

		public AudioSource _as;
		public AudioClip gunfire;

		//Drag in the Bullet Prefab from the Component Inspector.
		public GameObject Bullet;

		public Vector3 move;

		private float Delay;
		private int shotCounter;

		//Enter the Speed of the Bullet from the Component Inspector.
		public float Bullet_Forward_Force;
		private ThirdPersonUserControl m_Character;
		private Animator anim;
		// Use this for initialization
		void Start ()
		{
			m_Character = GetComponent<ThirdPersonUserControl>();
			anim = GetComponent<Animator> ();
			_as = GetComponent<AudioSource> ();

			shotangle = 1f;

			Delay = 0;
			shotCounter = 0;
		}

		// Update is called once per frame
		void Update ()
		{
			if (!anim.GetBool ("Death")) {
				Delay -= Time.deltaTime;
				if (CrossPlatformInputManager.GetButtonDown ("Fire3") && (Delay <= 0)) {
					//The Bullet instantiation happens here.
					GameObject Temporary_Bullet_Handler;
					Vector3 tempPosition = new Vector3 (72, Bullet_Emitter.transform.position.y, Bullet_Emitter.transform.position.z);
					if (m_Character.crouch && (Mathf.Abs (m_Character.m_Move.z) > 0))
						Temporary_Bullet_Handler = Instantiate (Bullet, tempPosition, Bullet_Emitter.transform.rotation) as GameObject;
					else
						Temporary_Bullet_Handler = Instantiate (Bullet, Bullet_Emitter.transform.position, Bullet_Emitter.transform.rotation) as GameObject;
					//Retrieve the Rigidbody component from the instantiated Bullet and control it.
					Rigidbody Temporary_RigidBody;
					Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody> ();

					move = m_Character.m_Move;

					if (m_Character.aimup) {
						move.y = shotangle;
						move.x = 0;
						//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
						Vector3 bulletforcecorrected = (move * Bullet_Forward_Force) / 2;
						Temporary_RigidBody.AddForce (bulletforcecorrected);
					}

					if (m_Character.crouch && (Mathf.Abs (m_Character.m_Move.z) > 0)) {
						move.y = -0.35f;
						move.x = 0;
						//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
						Vector3 bulletforcecorrected = (move * Bullet_Forward_Force) * 1.5f;
						Temporary_RigidBody.AddForce (bulletforcecorrected);
					} else
						Temporary_RigidBody.AddForce (transform.forward * Bullet_Forward_Force);

					_as.PlayOneShot (gunfire);

					//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
					Destroy (Temporary_Bullet_Handler, 2f);

					shotCounter++;

					if (shotCounter > 5) {
						Delay = 1.5f;
						shotCounter = 0;
					}
				}
			}
		}
	}
}