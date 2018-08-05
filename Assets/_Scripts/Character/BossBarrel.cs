using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBarrel : MonoBehaviour {

	private int hits;
	// Use this for initialization
	void Start () {
		hits = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (hits > 10)
			this.gameObject.SetActive (false);
			
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("MaryAnnBullet")) {
			other.gameObject.SetActive (false);
			hits++;
		}
	}
}
