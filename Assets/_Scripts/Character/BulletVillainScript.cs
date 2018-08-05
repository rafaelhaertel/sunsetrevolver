using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletVillainScript : MonoBehaviour {

	Rigidbody bullet_Rigidbody;
	// Use this for initialization
	void Start () {
		bullet_Rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("MaryAnn")) {
			bullet_Rigidbody.gameObject.SetActive (false);
			EnergyBarScript.energy = EnergyBarScript.energy - 25;
			if (EnergyBarScript.energy <= 1)
				other.gameObject.transform.parent.gameObject.GetComponent<Animator> ().SetBool ("Death", true);

		}
		if (other.gameObject.CompareTag ("CrashBullet")) {
			bullet_Rigidbody.gameObject.SetActive (false);
		}
	}
}