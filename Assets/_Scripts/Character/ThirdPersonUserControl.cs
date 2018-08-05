using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson1
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
		private Animator m_animator;
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        public Vector3 m_Move;
       	public bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
		public bool aimup;
		public bool crouch;
		public bool slide;
		public bool dash;
		public AudioSource _as;
		public AudioClip hurt;
		public AudioClip coin;
        
        private void Start()
        {	
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
			m_animator = GetComponent<Animator> ();
			_as = GetComponent<AudioSource> ();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Fire1");
            }

			if (!slide)
			{
				slide = CrossPlatformInputManager.GetButtonDown("Fire2");
			}
				
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
			float h = Mathf.Round( CrossPlatformInputManager.GetAxisRaw("Horizontal"));
			float v = 0;


			if(Input.GetAxisRaw("Vertical")< 0 )
            	crouch = true;

			if (Input.GetAxisRaw("Vertical") == 0) {
				crouch = false;
				aimup = false;
			}
				
			if(Input.GetAxisRaw("Vertical")> 0 )
				aimup = true;

			if (aimup && (h == 0))
				v = 1f;

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
				//m_Move = h*m_Cam.right;
				m_Move = v*m_CamForward + h*m_Cam.right;
			}
			else
			{
				// we use world-relative directions in the case of no main camera
				//m_Move = h*Vector3.right;
				m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

			if (m_animator.GetCurrentAnimatorStateInfo (0).IsName ("Dash"))
				dash = true;


            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump, aimup, slide, dash);
            m_Jump = false;
			slide = false;
			dash = false;
        }

		IEnumerator OnTriggerEnter(Collider other) {
			if (other.gameObject.CompareTag ("BulletVillain")) {
				_as.PlayOneShot (hurt, 0.5f);
			}

			if (other.gameObject.CompareTag ("Coin")) {
				_as.PlayOneShot (coin, 0.5f);
				EnergyBarScript.points = EnergyBarScript.points + 5000;
				if(EnergyBarScript.energy < 100)
					EnergyBarScript.energy = EnergyBarScript.energy + 5;
				other.gameObject.SetActive (false);
			}

			if (other.gameObject.CompareTag ("VillainHide")) {
				yield return new WaitForSeconds(3);
				other.gameObject.transform.GetChild (0).gameObject.SetActive (true);
				
			}
		}
    }
}
