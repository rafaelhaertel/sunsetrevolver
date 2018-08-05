using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

	GameObject bullet;
	// Use this for initialization
	void Start () {
		bullet = GetComponent<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("Villain")) {
			other.gameObject.transform.parent.gameObject.GetComponent<Animator> ().SetBool("Death", true);
			EnergyBarScript.points = EnergyBarScript.points + 1000;
			other.gameObject.GetComponent<BoxCollider> ().enabled = false;
			if (other.gameObject.transform.parent.transform.Find ("Coin_01"))
				other.gameObject.transform.parent.transform.Find("Coin_01").gameObject.SetActive (true);
			this.transform.gameObject.SetActive (false);
		}
		if (other.gameObject.CompareTag ("CrashBullet")) {
			this.transform.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("Boss")) {
			EnergyBarScriptBoss.energy = EnergyBarScriptBoss.energy - 5;
			this.transform.gameObject.SetActive (false);
			if (EnergyBarScriptBoss.energy <= 0)
				other.gameObject.transform.parent.gameObject.GetComponent<Animator> ().SetBool("Death", true);
		}
	}
}
