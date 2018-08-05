using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson1
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;

		Rigidbody m_Rigidbody;
		BoxCollider m_box;
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		public float m_ForwardAmount;
		public Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;
		public bool m_AimUp;
		public Vector3 movetest;
		public bool twoLevelJump;
		public bool changePlatform;
		public bool jumpController;
		public bool slideController;
		public Vector3 forceAir;
		public float airpush;
		public Vector3 moveRaw;


		void Start()
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;
			m_box = GetComponentInChildren<BoxCollider> ();
			airpush = 2;

			//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}


		public void Move(Vector3 move, bool crouch, bool jump, bool aimup, bool slide, bool dash)
		{
			if (!m_Animator.GetBool ("Death")) {
				jumpController = jump;
				slideController = slide;
				// convert the world relative moveInput vector into a local-relative
				// turn amount and forward amount required to head in the desired
				// direction.
				moveRaw = move;
				if (move.magnitude > 1f)
					move.Normalize ();
				move = transform.InverseTransformDirection (move);
				CheckGroundStatus ();



				move = Vector3.ProjectOnPlane (move, m_GroundNormal);

				if (m_GroundNormal.y < 0.97f) {
					move.y = move.y * 1.1f;
					move.z = move.z * 1.1f;
				}
				m_TurnAmount = Mathf.Atan2 (move.x, move.z);

				movetest = move;

				if (aimup && (moveRaw.z < 0f && moveRaw.z > -0.1f))
					move = Vector3.zero;

				m_ForwardAmount = move.z;

				forceAir = m_Rigidbody.velocity;

				ApplyExtraTurnRotation ();

				if (((twoLevelJump && jump) && aimup) && m_IsGrounded)
					changePlatform = true;

				// control and velocity handling is different when grounded and airborne:
				if (m_IsGrounded) {
					HandleGroundedMovement (crouch, jump);
				} else {
					HandleAirborneMovement (changePlatform, moveRaw);
				}

				m_Animator.SetBool ("AimUp", aimup);
				if (slide && m_IsGrounded && !dash)
					m_Animator.SetTrigger ("Slide");

				ScaleCapsuleForCrouching (crouch, dash);
				PreventStandingInLowHeadroom ();
				// send input and other state parameters to the animator
				UpdateAnimator (move);
			}
		}


		void ScaleCapsuleForCrouching(bool crouch, bool dash)
		{
			if (dash) {
				Vector3 boxColliderSize = new Vector3 (m_box.size.x, 0.36f, m_box.size.z);
				Vector3 boxColliderCenter = new Vector3 (m_box.center.x, 0.27f, m_box.center.z);
				m_box.size = boxColliderSize;
				m_box.center = boxColliderCenter;
			
			} else if (m_IsGrounded && crouch)
			{
				Vector3 boxColliderSize = new Vector3 (m_box.size.x, 1.11f, m_box.size.z);
				Vector3 boxColliderCenter = new Vector3 (m_box.center.x, 0.67f, m_box.center.z);
				m_box.size = boxColliderSize;
				m_box.center = boxColliderCenter;
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;

			}
			else
			{
				Vector3 boxColliderSize = new Vector3 (m_box.size.x, 1.47f, m_box.size.z);
				Vector3 boxColliderCenter = new Vector3 (m_box.center.x, 0.88f, m_box.center.z);
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
				m_box.size = boxColliderSize;
				m_box.center = boxColliderCenter;
				dash = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);

			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded);
			if (!m_IsGrounded)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded)
			{
				m_Animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;	
			}
		}

		void OnTriggerEnter(Collider other) {
			if (other.gameObject.CompareTag ("TwoLevel")) {
				twoLevelJump = true;
			}
		}

		void OnTriggerExit(Collider other) {
			if (other.gameObject.CompareTag ("TwoLevel")) {
				twoLevelJump = false;
				m_Rigidbody.velocity = new Vector3 (10f, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
				if (m_Rigidbody.position.y > 5) {
					m_Rigidbody.velocity = new Vector3 (10f, 4f, m_Rigidbody.velocity.z);
					m_IsGrounded = false;
					m_Animator.applyRootMotion = false;
					m_GroundCheckDistance = 0.1f;
				}
			}
		}


		void HandleAirborneMovement(bool ChangePlat, Vector3 moveRaw)
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			if (Mathf.Abs (m_Rigidbody.velocity.z) < 4) {
				moveRaw = new Vector3 (0, moveRaw.y, moveRaw.z * airpush);
				m_Rigidbody.AddForce (moveRaw);
			}

//			if(twoLevelJump)
				m_Rigidbody.velocity = new Vector3 (m_Rigidbody.velocity.x, m_Rigidbody.velocity.y, Mathf.Abs(m_Rigidbody.velocity.z) * moveRaw.z);
//			else 
//			if(moveRaw.x < 0 && !twoLevelJump)
//					m_Rigidbody.velocity = new Vector3 (0.02f, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);


//			if ((int)m_Rigidbody.velocity.z == 0) {
//				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;
//
//				// we preserve the existing y part of the current velocity.
//				v.y = m_Rigidbody.velocity.y;
//				v.x = 0f;
//				m_Rigidbody.velocity = v;
//			}



			if (ChangePlat) {
				m_Rigidbody.velocity = new Vector3 (-6f, m_Rigidbody.velocity.y + 2.5f, m_Rigidbody.velocity.z);
				changePlatform = false;
			}			

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
//			if (jump && !crouch && (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("AimUp")))
			if ((twoLevelJump && jump) && crouch) {
				m_Rigidbody.velocity = new Vector3 (6f, 6f, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
			else if (jump)
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}


		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				v.x = 0f;
				m_Rigidbody.velocity = v;
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}
